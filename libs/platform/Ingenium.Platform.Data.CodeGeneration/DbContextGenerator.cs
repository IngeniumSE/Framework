// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Ingenium.Platform.Data.CodeGeneration
{
	using System.Collections.Generic;
	using System.Text;

	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Text;

	/// <summary>
	/// Provides code generation for EF DbContext instances.
	/// </summary>
	[Generator]
	public class DbContextGenerator : ISourceGenerator
	{
		const string AttributeDefinition = @"
namespace Ingenium.Platform.Data.CodeGeneration
{
	using System;

	/// <summary>
	/// Marks a class for DbContext code generation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	sealed class GenerateDbContextAttribute : Attribute
	{
		/// <summary>
		/// Initialises a new instance of <see cref=""Ingenium.CodeGeneration.GenerateIdAttribute"" />
		/// <summary>
		/// <param name=""backingType"">The backing type of the struct.</param>
		public GenerateDbContextAttribute() { }
	}
}";

		static readonly DiagnosticDescriptor MissingAttributeErrror = new DiagnosticDescriptor(
			id: "INGPLATDATACGEN001",
			title: "Dynamically generated DbContext attribute could not be resolved",
			messageFormat: "The code generation item '{0}' could not be resolved in the current compilation",
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

			context.AddSource("GenerateDbContextAttribute.cs", attributeSourceText);

			var options = ((CSharpCompilation)context.Compilation)
				.SyntaxTrees[0].Options as CSharpParseOptions;

			var compilation = context.Compilation.AddSyntaxTrees(
				CSharpSyntaxTree.ParseText(attributeSourceText, options));

			var attributeSymbol = compilation.GetTypeByMetadataName("Ingenium.Platform.Data.CodeGeneration.GenerateDbContextAttribute");
			if (attributeSymbol is null)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(MissingAttributeErrror,
						Location.None,
						"Ingenium.Platform.Data.CodeGeneration.GenerateDbContextAttribute")
				);

				return;
			}

			// Stuff
		}

		/// <inheritdoc />
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
		}

		class SyntaxReceiver : ISyntaxReceiver
		{
			/// <summary>
			/// Gets the candidate set of class declarations.
			/// </summary>
			public List<ClassDeclarationSyntax> Candidates { get; } = new List<ClassDeclarationSyntax>();

			/// <inheritdoc />
			public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
			{
				if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
						classDeclarationSyntax.AttributeLists.Count > 0)
				{
					Candidates.Add(classDeclarationSyntax);
				}
			}
		}
	}
}
