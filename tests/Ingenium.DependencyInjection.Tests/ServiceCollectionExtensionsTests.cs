using System.Collections.ObjectModel;

using Ingenium.Modules;
using Ingenium.Parts;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using Xunit;

namespace Ingenium.DependencyInjection;

/// <summary>
/// Provides tests for the <see cref="ServiceCollectionExtensions"/> type.
/// </summary>
public class ServiceCollectionExtensionsTests
{
	[Fact]
	public void AddModuleServices_Should_ValidateArguments()
	{
		// Arrange
		var services = new ServiceCollection();
		var context = CreateContext();
		// Act, Assert
		Assert.Throws<ArgumentNullException>(
			"services",
			() => ServiceCollectionExtensions.AddModuleServices(
				null! /* services */,
				context));

		Assert.Throws<ArgumentNullException>(
			"context",
			() => ServiceCollectionExtensions.AddModuleServices(
				services,
				null! /* context */));
	}

	[Fact]
	public void AddModuleServices_Should_CallAddServices_ForEachModule_ThatImplementsInterface()
	{
		// Arrange
		ServicesBuilderContext? capturedContext = default;
		IServiceCollection? capturedServices = default;
		int calls = 0;

		var services = new ServiceCollection();
		var modules = CreateModuleProvider((context, services)
			=>
		{
			capturedContext = context;
			capturedServices = services;
			calls++;
		});
		var context = CreateContext(modules.Object);

		// Act
		ServiceCollectionExtensions.AddModuleServices(services, context);

		// Assert
		modules.Verify();

		Assert.Same(capturedContext, context);
		Assert.Same(capturedServices, services);
		Assert.Equal(1, calls);
	}

	ServicesBuilderContext CreateContext(IModuleProvider? modules = default)
		=> new ServicesBuilderContext(
			Mock.Of<IConfiguration>(),
			Mock.Of<IHostEnvironment>(),
			modules ?? Mock.Of<IModuleProvider>(),
			Mock.Of<IPartManager>());
	Mock<IModuleProvider> CreateModuleProvider(Action<ServicesBuilderContext, IServiceCollection> onCalled)
	{
		var mock = new Mock<IModuleProvider>();

		mock.Setup(x => x.Modules)
			.Returns(new ReadOnlyCollection<IModule>(
					new List<IModule> { new TestModule(onCalled), new OtherModule() }
				)
			)
			.Verifiable();

		return mock;
	}

	class TestModule : Module, IServicesBuilder
	{
		readonly Action<ServicesBuilderContext, IServiceCollection> _onCalled;

		public TestModule(Action<ServicesBuilderContext, IServiceCollection> onCalled)
			=> _onCalled = onCalled;

		void IServicesBuilder.AddServices(ServicesBuilderContext context, IServiceCollection services)
			=> _onCalled(context, services);
	}
	class OtherModule : Module { }
}