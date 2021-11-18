using System.Reflection;

using Ingenium.Parts;

using Xunit;

namespace Ingenium.Modules;

/// <summary>
/// Provides tests for the <see cref="ModulePartFeatureProvider"/> type.
/// </summary>
public class ModulePartFeatureProviderTests
{
	[Fact]
	public void PopulateFeature_Should_ValidateArguments()
	{
		// Arrange
		var provider = new ModulePartFeatureProvider();
		var feature = new ModulePartFeature();
		var parts = Enumerable.Empty<IPart>();

		// Act, Assert
		Assert.Throws<ArgumentNullException>(
			"parts",
			() => provider.PopulateFeature(
				null! /* parts */,
				feature));

		Assert.Throws<ArgumentNullException>(
			"feature",
			() => provider.PopulateFeature(
				parts,
				null! /* feature */));
	}

	[Fact]
	public void PopulateFeature_Should_PopulateFeature_FromAvailableModules()
	{
		// Arrange
		var provider = new ModulePartFeatureProvider();
		var feature = new ModulePartFeature();
		var parts = new IPart[] { new AssemblyPart(typeof(ModuleTests).Assembly) };

		// Act
		provider.PopulateFeature(parts, feature);

		// Assert
		Assert.NotNull(feature.ModuleTypes);
		Assert.Equal(2, feature.ModuleTypes.Count);
		Assert.Contains(feature.ModuleTypes, mt => mt == typeof(TestModule).GetTypeInfo());
		Assert.Contains(feature.ModuleTypes, mt => mt == typeof(OtherModule).GetTypeInfo());
	}
}