using Moq;

namespace Ingenium.DependencyInjection;

/// <summary>
/// Provides tests for the <see cref="ProviderServiceFactory{TService}"/> type.
/// </summary>
public class ProviderServiceFactoryTests
{
	[Fact]
	public void GetProviderService_ReturnsInstance_ForKnownProviderId()
	{
		// Arrange
		var services = CreateServiceProvider("Hello");
		var providers = new[]
		{
			new Provider<string>(new ProviderId("Test"), typeof(string))
		};
		var factory = new ProviderServiceFactory<string>(
			services.Object,
			providers);

		// Act
		var service = factory.GetProviderService(new ProviderId("Test"));

		// Assert
		Assert.Equal("Hello", service);
	}

	[Fact]
	public void GetProviderService_ThrowsException_ForUnknownProviderId()
	{
		// Arrange
		var services = CreateServiceProvider("Hello");
		var providers = new[]
		{
			new Provider<string>(new ProviderId("Test"), typeof(string))
		};
		var factory = new ProviderServiceFactory<string>(
			services.Object,
			providers);

		// Act, Assert
		Assert.Throws<ArgumentException>(
			() => factory.GetProviderService(new ProviderId("Other")));
	}

	Mock<IServiceProvider> CreateServiceProvider<TService>(TService instance)
	{
		var mock = new Mock<IServiceProvider>();

		mock.Setup(m => m.GetService(It.IsAny<Type>()))
				.Returns(instance);

		return mock;
	}
}
