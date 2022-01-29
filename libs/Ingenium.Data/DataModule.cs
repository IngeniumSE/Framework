using Ingenium.DependencyInjection;
using Ingenium.Modules;

using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.Data;

/// <summary>
/// Allows composition of data services into an application.
/// </summary>
[Module(
	id: "Data",
	name: "Data",
	description: "Provides data services for applications.")]
public class DataModule : Module, IServicesBuilder
{
	/// <inheritdoc />
	public void AddServices(ServicesBuilderContext context, IServiceCollection services)
	{
		Ensure.IsNotNull(context, nameof(context));
		Ensure.IsNotNull(services, nameof(services));

		services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
		services.AddSingleton<ITransactionManager, TransactionManager>();
		services.AddScoped<ISqlFactory, DapperSqlFactory>();
		services.AddScoped(s => s.GetRequiredService<ISqlFactory>().CreateSqlContext());

		services.Configure<DataOptions, DataOptionsValidator>(
			context.Configuration.GetSection(DataOptions.ConfigurationSectionKey));
	}
}