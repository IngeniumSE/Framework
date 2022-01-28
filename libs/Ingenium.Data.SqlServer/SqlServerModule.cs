using Ingenium.DependencyInjection;
using Ingenium.Modules;

using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.Data.SqlServer;

/// <summary>
/// Allows composition of Sql Server data services into an application.
/// </summary>
[Module(
	id: "Data.SqlServer",
	name: "Data: Sql Server",
	description: "Provides Sql Server data services for applications.")]
public class SqlServerModule : Module, IServicesBuilder
{
	/// <inheritdoc />
	public void AddServices(ServicesBuilderContext context, IServiceCollection services)
	{
		Ensure.IsNotNull(services, nameof(services));

		services.AddScopedProviderService<IConnectionContextFactory, SqlServerConnectionContextFactory>();
	}
}