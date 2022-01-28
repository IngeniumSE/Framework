namespace Ingenium.CodeGeneration
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;

	using VerifyXunit;

	public static class SourceGeneratorTestHelper
	{
		public static Task Verify(string source)
		{
			var tree = CSharpSyntaxTree.ParseText(source);
			var references = new[]
			{
				MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(IdGenerator).Assembly.Location)
			};
			var compilation = CSharpCompilation.Create(
				assemblyName: "Tests",
				syntaxTrees: new[] { tree },
				references: references);

			var generator = new IdGenerator();
			GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

			driver = driver.RunGenerators(compilation);

			return Verifier.Verify(driver).UseDirectory("Snapshots");
		}
	}
}
