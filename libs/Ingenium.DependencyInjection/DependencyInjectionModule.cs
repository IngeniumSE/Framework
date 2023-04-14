using Ingenium.Modules;

using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.DependencyInjection;

/// <summary>
/// Allows composition of data services into an application.
/// </summary>
[Module(
	id: "DependencyInjection",
	name: "DependencyInjection",
	description: "Provides DI services for applications.")]
public class DependencyInjectionModule : Module, IServicesBuilder
{
	/// <inheritdoc />
	public void AddServices(ServicesBuilderContext context, IServiceCollection services)
	{
		Ensure.IsNotNull(services, nameof(services));

		services.AddTransient(typeof(IProviderServiceFactory<>), typeof(ProviderServiceFactory<>));
	}
}