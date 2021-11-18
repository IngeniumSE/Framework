using Ingenium.Configuration;
using Ingenium.DependencyInjection;
using Ingenium.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.Hosting;

/// <summary>
/// Allows composition of hosting services into an application.
/// </summary>
[Module(
	id: "Hosting",
	name: "Hosting",
	description: "Provides hosting services for applications.")]
public class HostingModule : Module, IServicesBuilder, IConfigurationExtender
{
	/// <inheritdoc />
	public void AddConfiguration(ConfigurationBuilderContext context, IConfigurationBuilder builder)
	{
		// Add the root configuration files.
		builder.AddJsonFile("./appsettings.json", optional: true);
		builder.AddJsonFile("./appsettings.env.json", optional: true);
	}

	/// <inheritdoc />
	public void AddServices(ServicesBuilderContext context, IServiceCollection services)
	{
		services.AddSingleton<IAppServices, AppServices>();
	}
}