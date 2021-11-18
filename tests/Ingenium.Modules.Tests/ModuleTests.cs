using Xunit;

namespace Ingenium.Modules;

/// <summary>
/// Provides tests for the <see cref="Module"/> type.
/// </summary>
public class ModuleTests
{
	[Fact]
	public void Constructor_Should_GenerateDescriptor_WhenNoAttributeProvided()
	{
		// Arrange, Act
		var module = new TestModule();

		// Assert
		Assert.NotNull(module.Descriptor);
		Assert.Equal("Test", module.Descriptor.Id.Value);
		Assert.Null(module.Descriptor.Name);
		Assert.Null(module.Descriptor.Description);
		Assert.Null(module.Descriptor.Dependencies);
	}

	[Fact]
	public void Constructor_Should_CreateDescriptor_FromAttribute_WhenProvided()
	{
		// Arrange, Act
		var module = new OtherModule();

		// Assert
		Assert.NotNull(module.Descriptor);
		Assert.Equal("CustomModuleId", module.Descriptor.Id.Value);
		Assert.Equal("Other Module", module.Descriptor.Name);
		Assert.Equal("An example module using a ModuleAttribute", module.Descriptor.Description);
		Assert.NotNull(module.Descriptor.Dependencies);
		Assert.Collection(
			module.Descriptor.Dependencies, 
			 d => Assert.Equal("Test", d.Value));
	}
}