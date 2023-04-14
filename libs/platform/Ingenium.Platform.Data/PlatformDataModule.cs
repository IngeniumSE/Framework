using Ingenium.DependencyInjection;
using Ingenium.Modules;

using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.Platform.Data;

/// <summary>
/// Allows composition of platform data services into an application.
/// </summary>
[Module(
	id: "Platform.Data",
	name: "Platform.Data",
	description: "Provides platform data services for applications.")]
public class PlatformDataModule : Module, IServicesBuilder
{
	/// <inheritdoc />
	public void AddServices(ServicesBuilderContext context, IServiceCollection services)
	{
		Ensure.IsNotNull(context, nameof(context));
		Ensure.IsNotNull(services, nameof(services));
	}
}