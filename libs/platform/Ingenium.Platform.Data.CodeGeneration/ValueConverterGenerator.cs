// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Ingenium.Platform.Data.CodeGeneration
{
using System;
	using System.Collections.Generic;
	using System.Text;

	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Text;

	/// <summary>
	/// Provides code generation for EF ValueConverter instances.
	/// </summary>
	[Generator]
	public class ValueConverterGenerator : ISourceGenerator
	{
		const string AttributeDefinition = @"
namespace Ingenium.Platform.Data.CodeGeneration
{
	using System;

	/// <summary>
	/// Marks a class for ValueConverter code generation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	sealed class GenerateValueConverterAttribute : Attribute
	{
		/// <summary>
		/// Initialises a new instance of <see cref=""Ingenium.CodeGeneration.GenerateValueConverterAttribute"" />
		/// <summary>
		/// <param name=""backingType"">The backing type of the struct.</param>
		public GenerateValueConverterAttribute() { }
	}
}";

		static readonly DiagnosticDescriptor MissingAttributeErrror = new DiagnosticDescriptor(
			id: "INGPLATDATACGEN002",
			title: "Dynamically generated ValueConverter attribute could not be resolved",
			messageFormat: "The code generation item '{0}' could not be resolved in the current compilation",
			category: "IngeniumPlatformDataCodeGeneration",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		static readonly DiagnosticDescriptor MissingGenerateIdAttributeErrror = new DiagnosticDescriptor(
			id: "INGPLATDATACGEN003",
			title: "ValueConverter generator required a Generated ID",
			messageFormat: "To generate a value converter from an auto-generated ID, the [GenerateId] attribute must also be applied to the type",
			category: "IngeniumPlatformDataCodeGeneration",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		/// <inheritdoc />
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			{
				return;
			}

			var attributeSourceText = SourceText.From(AttributeDefinition, Encoding.UTF8);

			context.AddSource("GenerateValueConverterAttribute.cs", attributeSourceText);

			var options = ((CSharpCompilation)context.Compilation)
				.SyntaxTrees[0].Options as CSharpParseOptions;

			var compilation = context.Compilation.AddSyntaxTrees(
				CSharpSyntaxTree.ParseText(attributeSourceText, options));

			var attributeSymbol = compilation.GetTypeByMetadataName("Ingenium.Platform.Data.CodeGeneration.GenerateValueConverterAttribute");
			if (attributeSymbol is null)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(MissingAttributeErrror,
						Location.None,
						"Ingenium.Platform.Data.CodeGeneration.GenerateValueConverterAttribute")
				);

				return;
			}

			var symbols = new List<(SemanticModel, INamedTypeSymbol)>();
			foreach (var candidate in receiver.Candidates)
			{
				var model = compilation.GetSemanticModel(candidate.SyntaxTree);
				var str = model.GetDeclaredSymbol(candidate);
				if (str is not null && str.GetAttributes().Any(ad => ad.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
				{
					symbols.Add((model, str));
				}
			}

			foreach (var (model, symbol) in symbols)
			{
				var generateIdAttribute = symbol.GetAttributes().FirstOrDefault(ad =>
					string.Equals(ad.AttributeClass!.Name, "GenerateIdAttribute", StringComparison.Ordinal));
				if (generateIdAttribute is null)
				{
					context.ReportDiagnostic(Diagnostic.Create(MissingGenerateIdAttributeErrror, symbol.Locations[0]));

					continue;
				}

				var typeArg = generateIdAttribute.ConstructorArguments.Length > 0
					? generateIdAttribute.ConstructorArguments[0].Value as INamedTypeSymbol
					: default;

				if (typeArg is null)
				{
					typeArg = context.Compilation.GetTypeByMetadataName("System.Int32")!;
				}

				string source = ProcessClass(symbol, attributeSymbol, typeArg, context);
				context.AddSource($"{symbol.Name}_ValueGeneratorImpl.generated.cs", SourceText.From(source, Encoding.UTF8));
			}
		}

		/// <inheritdoc />
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
		}

		string ProcessClass(
			INamedTypeSymbol typeSymbol, 
			INamedTypeSymbol attributeSymbol, 
			INamedTypeSymbol backingTypeSymbol,
			GeneratorExecutionContext context)
		{
			string ns = typeSymbol.ContainingNamespace.ToDisplayString();
			string name = typeSymbol.Name;

			string source = $@"namespace {ns}
{{
	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	public class {name}ValueConverter : ValueConverter<{name}, {backingTypeSymbol.Name}>
  {{
		public {name}ValueConverter()
      : base(v => v.Value, v => new {name}(v)) {{ }}
	}}
}}";

			return source;
		}

		class SyntaxReceiver : ISyntaxReceiver
		{
			/// <summary>
			/// Gets the candidate set of struct declarations.
			/// </summary>
			public List<StructDeclarationSyntax> Candidates { get; } = new List<StructDeclarationSyntax>();

			/// <inheritdoc />
			public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
			{
				if (syntaxNode is StructDeclarationSyntax structDeclarationSyntax &&
						structDeclarationSyntax.AttributeLists.Count > 0)
				{
					Candidates.Add(structDeclarationSyntax);
				}
			}
		}
	}
}
