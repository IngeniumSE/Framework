using Ingenium.Modules;

using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.DependencyInjection;

/// <summary>
/// Provides extensions for the <see cref="IServiceCollection"/> type.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers any modules services with the service collection.
	/// </summary>
	/// <param name="services">The service collection.</param>
	/// <param name="context">The services builder context.</param>
	/// <returns>The service collection.</returns>
	public static IServiceCollection AddModuleServices(
		this IServiceCollection services,
		ServicesBuilderContext context)
	{
		Ensure.IsNotNull(services, nameof(services));
		Ensure.IsNotNull(context, nameof(context));

		services.AddSingleton(context.Modules);

		foreach (var builder in context.Modules.EnumerateModules<IServicesBuilder>())
		{
			builder.AddServices(context, services);
		}

		return services;
	}
}
