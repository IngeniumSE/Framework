using System.Transactions;

using Ingenium.DependencyInjection;
using Ingenium.Modules;

using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.Gql;

/// <summary>
/// Allows composition of data services into an application.
/// </summary>
[Module(
	id: "GraphQL",
	name: "GraphQL",
	description: "Provides GraphQL services for applications.")]
public class GqlModule : Module, IServicesBuilder
{
	/// <inheritdoc />
	public void AddServices(ServicesBuilderContext context, IServiceCollection services)
	{
		Ensure.IsNotNull(context, nameof(context));
		Ensure.IsNotNull(services, nameof(services));

		services.Configure<GqlOptions, GqlOptionsValidator>(
			context.Configuration.GetSection(GqlOptions.ConfigurationSectionKey));

		services.AddScoped<RootSchema>();
		services.AddScoped<IRootQueryFactory, RootQueryFactory>();
	}
}