﻿// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Ingenium.CodeGeneration
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Text;

	/// <summary>
	/// Provides code generation for immutable structs.
	/// </summary>
	[Generator]
	public class StructGenerator : ISourceGenerator
	{
		const string AttributeDefinition = @"
namespace Ingenium.CodeGeneration
{
	using System;

	/// <summary>
	/// Marks a struct for code generation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	sealed class GenerateStructAttribute : Attribute
	{
		/// <summary>
		/// Initialises a new instance of <see cref=""Alium.CodeGeneration.GenerateStructAttribute"" />
		/// <summary>
		/// <param name=""backingType"">The backing type of the struct.</param>
		public GenerateStructAttribute(Type? backingType = default)
		{
			BackingType = backingType is object ? backingType : typeof(int);
		}

		/// <summary>
		/// Gets the backing type for the struct.
		/// </summary>
		public Type BackingType { get; }
	}
}";

		static readonly DiagnosticDescriptor MissingAttributeErrror = new DiagnosticDescriptor(
			id: "INGCGEN001",
			title: "Dynamically generated struct attribute could not be resolved",
			messageFormat: "The code generation item '{0}' could not be resolved in the current compilation",
			category: "AliumCodeGeneration",
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

			context.AddSource("GenerateStructAttribute.cs", attributeSourceText);

			var options = ((CSharpCompilation)context.Compilation)
				.SyntaxTrees[0].Options as CSharpParseOptions;

			var compilation = context.Compilation.AddSyntaxTrees(
				CSharpSyntaxTree.ParseText(attributeSourceText, options));

			var attributeSymbol = compilation.GetTypeByMetadataName("Ingenium.CodeGeneration.GenerateStructAttribute");
			if (attributeSymbol is null)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(MissingAttributeErrror,
						Location.None,
						"Ingenium.CodeGeneration.GenerateStructAttribute")
				);

				return;
			}

			var symbols = new List<INamedTypeSymbol>();
			foreach (var candidate in receiver.Candidates)
			{
				var model = compilation.GetSemanticModel(candidate.SyntaxTree);
				var str = model.GetDeclaredSymbol(candidate);
				if (str is not null && str.GetAttributes().Any(ad => ad.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
				{
					symbols.Add(str);
				}
			}

			foreach (var symbol in symbols)
			{
				string source = ProcessClass(symbol, attributeSymbol, context);
				context.AddSource($"{symbol.Name}_StructImpl.generated.cs", SourceText.From(source, Encoding.UTF8));
			}
		}

		/// <inheritdoc />
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

			//System.Diagnostics.Debugger.Launch();
		}

		string ArgumentValidation(bool isValueType, bool isStringType)
			=> isValueType
					? string.Empty
					: isStringType
						? $@"if (value is not {{ Length: >0 }}) {{ throw new ArgumentException(""The provided value must be a non-empty string."", nameof(value)); }}"
						: $@"if (value is null) {{ throw new ArgumentNullException(nameof(value)); }}";

		string EqualityCheck(bool isStringType, string valueSymbol)
			=> isStringType
					? $"Value.Equals({valueSymbol}, StringComparison.Ordinal)"
					: $"Value.Equals({valueSymbol})";

		bool IsStringType(INamedTypeSymbol symbol, GeneratorExecutionContext context)
		{
			var stringType = context.Compilation.GetTypeByMetadataName("System.String");
			return symbol.Equals(stringType, SymbolEqualityComparer.Default);
		}

		bool IsValueType(INamedTypeSymbol symbol, GeneratorExecutionContext context)
		{
			var valueType = context.Compilation.GetTypeByMetadataName("System.ValueType");
			return symbol.BaseType is not null &&
						 symbol.BaseType.Equals(valueType, SymbolEqualityComparer.Default);
		}

		string ProcessClass(INamedTypeSymbol typeSymbol, INamedTypeSymbol attributeSymbol, GeneratorExecutionContext context)
		{
			string ns = typeSymbol.ContainingNamespace.ToDisplayString();
			string name = typeSymbol.Name;

			var attribute = typeSymbol.GetAttributes().First(ad => ad.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default));
			var arg = attribute.ConstructorArguments.First().Value as INamedTypeSymbol;

			if (arg is null)
			{
				arg = context.Compilation.GetTypeByMetadataName("System.Int32")!;
			}
			bool argIsValueType = IsValueType(arg, context);
			bool argIsStringType = IsStringType(arg, context);

			string source = $@"namespace {ns}
{{
	using System;
	using System.Diagnostics;

	[DebuggerDisplay(""{{DebuggerToString(){(argIsStringType ? "" : ",nq")}}}"")]
	partial struct {name} : IComparable<{name}>, IEquatable<{name}>
	{{
		/// <summary>
		/// Represents an empty instance.
		/// </summary>
		public static readonly {name} Empty = new {name}();

		public {name}({arg.Name} value)
		{{
			{ArgumentValidation(argIsValueType, argIsStringType)}

			Value = value;
			HasValue = true;
		}}

		/// <summary>
		/// Gets whether the instance has a value.
		/// </summary>
		public bool HasValue {{ get; }}

		/// <summary>
		/// Gets the value.
		/// </summary>
		public {arg.Name} Value {{ get; }}

		/// <inheritdoc />
		public int CompareTo({name} other)
		{{
			if (HasValue && HasValue == other.HasValue)
			{{
				return Value.CompareTo(other.Value);
			}}

			return HasValue ? -1 : 1;
		}}

		/// <inheritdoc />
		public override bool Equals(object other)
			=> other switch
				{{
					{name} value => Equals(value),
					{arg.Name} value => Equals(value),
					_ => false
				}};

		/// <inheritdoc />
		public bool Equals({name} other)
		{{
			if (HasValue && HasValue == other.HasValue)
			{{
				return {EqualityCheck(argIsStringType, "other.Value")};
			}}

			return false;
		}}

		/// <inheritdoc />
		public bool Equals({arg.Name} other)
		{{
			if (HasValue)
			{{
				return {EqualityCheck(argIsStringType, "other")};
			}}

			return false;
		}}

		/// <inheritdoc />
		public override int GetHashCode()
			=> HashCode.Combine(HasValue, Value);

		/// <inheritdoc />
		public override string ToString()
			=> HasValue ? Value.ToString() : string.Empty;

		/// <inheritdoc />
		string DebuggerToString()
			=> HasValue ? Value.ToString() : ""(empty)"";

		public static bool operator ==({name} left, {name} right)
			=> left.Equals(right);
		public static bool operator !=({name} left, {name} right)
			=> !left.Equals(right);
	}}
}}";

			return source;
		}
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