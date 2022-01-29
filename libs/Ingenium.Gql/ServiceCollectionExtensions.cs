using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.Gql;

/// <summary>
/// Provides extensions for the <see cref="IServiceCollection"/> type.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds a root query extender to the service collection.
	/// </summary>
	/// <typeparam name="TExtender">The extender type.</typeparam>
	/// <param name="services">The service collection.</param>
	/// <returns>The service collection.</returns>
	public static IServiceCollection AddRootQueryExtender<TExtender>(
		this IServiceCollection services)
		where TExtender : class, IRootQueryExtender
	{
		Ensure.IsNotNull(services, nameof(services));

		return services.AddScoped<IRootQueryExtender, TExtender>();
	}

	/// <summary>
	/// Adds a root schema extender to the service collection.
	/// </summary>
	/// <typeparam name="TExtender">The extender type.</typeparam>
	/// <param name="services">The service collection.</param>
	/// <returns>The service collection.</returns>
	public static IServiceCollection AddRootSchemaExtender<TExtender>(
		this IServiceCollection services)
		where TExtender : class, IRootSchemaExtender
	{
		Ensure.IsNotNull(services, nameof(services));

		return services.AddScoped<IRootSchemaExtender, TExtender>();
	}
}
