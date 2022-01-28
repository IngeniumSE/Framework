using Ingenium.DependencyInjection;
using Ingenium.Modules;

using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.Storage.FileSystem;

/// <summary>
/// Allows composition of file system storage services into an application.
/// </summary>
[Module(
	id: "Storage.FileSystem",
	name: "Storage: File System",
	description: "Provides file system storage services for applications.")]
public class FileSystemStorageModule : Module, IServicesBuilder
{
	/// <inheritdoc />
	public void AddServices(ServicesBuilderContext context, IServiceCollection services)
	{
		Ensure.IsNotNull(context, nameof(context));
		Ensure.IsNotNull(services, nameof(services));

		services.AddScopedProviderService<IStorageProvider, FileSystemStorageProvider>();
		services.AddSingleton<IStorageProfileProvider, FileSystemStorageProfileProvider>();
	}
}