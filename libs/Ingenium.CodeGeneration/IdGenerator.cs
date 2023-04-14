using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Ingenium.CodeGeneration
{
	[Generator]
	public class IdGenerator : IIncrementalGenerator
	{
		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			IncrementalValuesProvider<StructDeclarationSyntax> candidates = context.SyntaxProvider
				.CreateSyntaxProvider(
					predicate: static (s, _) => IsSyntaxTargetForCodeGen(s),
					transform: static (ctx, _) => GetSemanticTargetForCodeGen(ctx)
				)
				.Where(static m => m is not null)!;

			IncrementalValueProvider<(Compilation, ImmutableArray<StructDeclarationSyntax>)> targets
				= context.CompilationProvider.Combine(candidates.Collect());

			context.RegisterSourceOutput(targets,
				static (spc, target) => Execute(target.Item1, target.Item2, spc));
		}

		static bool IsSyntaxTargetForCodeGen(SyntaxNode node)
			=> node is StructDeclarationSyntax { AttributeLists: { Count: > 0 } };

		static StructDeclarationSyntax? GetSemanticTargetForCodeGen(GeneratorSyntaxContext context)
		{
			var syntax = (StructDeclarationSyntax)context.Node;

			foreach (var list in syntax.AttributeLists)
			{
				foreach (var attribute in list.Attributes)
				{
					if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol method)
					{
						// Shouldn't hit here, but just in case.
						continue;
					}

					INamedTypeSymbol type = method.ContainingType;
					string fullName = type.ToDisplayString();

					if (string.Equals(SourceGenerationHelper.AttributeFullName, fullName, StringComparison.Ordinal))
					{
						return syntax;
					}
				}
			}

			return default;
		}

		static void Execute(Compilation compilation, ImmutableArray<StructDeclarationSyntax> structs, SourceProductionContext context)
		{
			if (structs.IsDefaultOrEmpty)
			{
				// No work to complete.
				return;
			}

			var distinct = structs.Distinct();
			var targets = GetTypesToGenerate(compilation, distinct, context.CancellationToken);

			if (targets.Count > 0)
			{
				foreach (var target in targets)
				{
					string fullName = target.Type.ToDisplayString();
					string result = SourceGenerationHelper.GenerateIdClass(target);

					context.AddSource($"{fullName}.g.cs", SourceText.From(result, Encoding.UTF8));
				}
			}
		}

		static List<StructToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<StructDeclarationSyntax> structs, CancellationToken cancellationToken)
		{
			var targets = new List<StructToGenerate>();
			var attributeDef = compilation.GetTypeByMetadataName(SourceGenerationHelper.AttributeFullName);
			if (attributeDef is null)
			{
				// Shouldn't happen, but safety first.
				return targets;
			}

			foreach(var @struct in structs)
			{
				cancellationToken.ThrowIfCancellationRequested();

				var model = compilation.GetSemanticModel(@struct.SyntaxTree);
				if (model.GetDeclaredSymbol(@struct) is not INamedTypeSymbol symbol)
				{
					// Something went wrong, skip this one.
					continue;
				}

				string name = symbol.ToString();
				var attribute = symbol.GetAttributes()
					.Single(ad => SymbolEqualityComparer.Default.Equals(ad.AttributeClass, attributeDef));

				var defaultBackingType = compilation.GetTypeByMetadataName("System.Int32");
				var backingType = (attribute.ConstructorArguments.Length > 0
					? attribute.ConstructorArguments.ElementAtOrDefault(0).Value as INamedTypeSymbol
					: defaultBackingType) ?? defaultBackingType;

#pragma warning disable CS8605 // Unboxing a possibly null value.
				bool caseInsensitive = bool.Parse(attribute.ConstructorArguments[1].Value!.ToString());
#pragma warning restore CS8605 // Unboxing a possibly null value.

				int featuresFlag = int.Parse(attribute.ConstructorArguments[2].Value!.ToString());
				var features = (GenerateIdFeatures)featuresFlag;

				targets.Add(new StructToGenerate(
					compilation,
					symbol,
					@struct,
					name,
					backingType!,
					caseInsensitive,
					features));
			}

			return targets;
		}

		internal class StructToGenerate
		{
			public StructToGenerate(
				Compilation compilation,
				INamedTypeSymbol type,
				StructDeclarationSyntax syntax,
				string fullName,
				INamedTypeSymbol backingType,
				bool caseInsensitive,
				GenerateIdFeatures features)
			{
				Compilation = compilation;
				Type = type;
				Syntax = syntax;
				FullName = fullName;
				BackingType = backingType;
				CaseInsensitive = caseInsensitive;
				Features = features;
			}

			public Compilation Compilation { get; }
			public INamedTypeSymbol Type { get; }
			public StructDeclarationSyntax Syntax { get; }
			public string FullName { get; }
			public INamedTypeSymbol BackingType { get; }
			public bool CaseInsensitive { get; }
			public GenerateIdFeatures Features { get; }
		}
	}
}
