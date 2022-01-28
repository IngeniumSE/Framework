namespace Ingenium.CodeGeneration
{
	using VerifyXunit;

	using Xunit;

	[UsesVerify]
	public class StructGeneratorVerification
	{
		[Fact]
		public Task GeneratesInt32StructSourceCodeCorrectly()
		{
			var source = @"
using Ingenium.CodeGeneration;

[GenerateId]
public partial struct TestId {}";

			return SourceGeneratorTestHelper.Verify(source);
		}
	}
}
 