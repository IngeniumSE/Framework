using Ingenium.DependencyInjection;
using Ingenium.Modules;

using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.Storage;

/// <summary>
/// Allows composition of storage services into an application.
/// </summary>
[Module(
	id: "Storage",
	name: "Storage",
	description: "Provides storage services for applications.")]
public class StorageModule : Module, IServicesBuilder
{
	/// <inheritdoc />
	public void AddServices(ServicesBuilderContext context, IServiceCollection services)
	{
		Ensure.IsNotNull(context, nameof(context));
		Ensure.IsNotNull(services, nameof(services));

		services.Configure<StorageOptions, StorageOptionsValidator>(
			context.Configuration.GetSection(StorageOptions.ConfigurationSectionKey));

		services.AddSingleton<IStorageOptionsProvider, StorageOptionsProvider>();
		services.AddScoped<IStorageFactory, StorageFactory>();
	}
}