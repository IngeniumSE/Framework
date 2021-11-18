using Xunit;

namespace Ingenium.Parts;

/// <summary>
/// Provides tests for the <see cref="PartManager"/> type.
/// </summary>
public class PartManagerTests
{
	[Fact]
	public void PopulateFeature_Should_ValidateArguments()
	{
		// Arrange
		var parts = new PartManager();

		// Act, Assert
		Assert.Throws<ArgumentNullException>(
			"feature",
			() => parts.PopulateFeature<object>(null! /* feature */));
	}

	[Fact]
	public void PopulateFeature_Should_UsePartFeatureProviders_ToPopulateFeature()
	{
		// Arrange
		var parts = new PartManager();
		parts.AddProvider(new TestPartFeatureProvider());
		parts.AddProvider(new OtherPartFeatureProvider());

		var feature = new TestPartFeature();

		// Act
		parts.PopulateFeature(feature);

		// Assert
		Assert.Equal("MATT", feature.Value);
	}

	class TestPartFeature
	{
		public string? Value { get; set; }
	}
	class TestPartFeatureProvider : IPartFeatureProvider<TestPartFeature>
	{
		public void PopulateFeature(IEnumerable<IPart> parts, TestPartFeature feature)
		{
			feature.Value = "MATT";
		}
	}

	class OtherPartFeature { }
	class OtherPartFeatureProvider : IPartFeatureProvider<OtherPartFeature>
	{
		public void PopulateFeature(IEnumerable<IPart> parts, OtherPartFeature feature)
		{
			throw new NotImplementedException();
		}
	}
}