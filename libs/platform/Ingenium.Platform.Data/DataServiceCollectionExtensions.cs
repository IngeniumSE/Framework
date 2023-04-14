using Ingenium.Data;
using Ingenium.Platform.Data;

using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extensions for the <see cref="IServiceCollection"/> type.
/// </summary>
public static class DataServiceCollectionExtensions
{
	/// <summary>
	/// Adds reader services for the given entity.
	/// </summary>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <typeparam name="TImplementation">The implementation type.</typeparam>
	/// <typeparam name="TEntity">The entity type.</typeparam>
	/// <typeparam name="TKey">The key type.</typeparam>
	/// <param name="services">The services collection.</param>
	/// <returns>The services collection.</returns>
	public static IServiceCollection AddEntityReader<TService, TImplementation, TEntity, TKey>(this IServiceCollection services)
		where TService : class
		where TImplementation : class, TService, IReader<TEntity, TKey>
		where TEntity : Entity
	{
		Ensure.IsNotNull(services, nameof(services));

		services.AddScoped<TImplementation>();
		services.AddTransient(sp => (TService)sp.GetRequiredService<TImplementation>());
		services.AddTransient(sp => (IReader<TEntity, TKey>)sp.GetRequiredService<TImplementation>());

		return services;
	}

	/// <summary>
	/// Adds reader services for the given entity.
	/// </summary>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <typeparam name="TImplementation">The implementation type.</typeparam>
	/// <typeparam name="TEntity">The entity type.</typeparam>
	/// <param name="services">The services collection.</param>
	/// <returns>The services collection.</returns>
	public static IServiceCollection AddEntityReader<TService, TImplementation, TEntity>(this IServiceCollection services)
		where TService : class
		where TImplementation : class, TService, IReader<TEntity>
		where TEntity : Entity
	{
		Ensure.IsNotNull(services, nameof(services));

		services.AddScoped<TImplementation>();
		services.AddTransient(sp => (TService)sp.GetRequiredService<TImplementation>());
		services.AddTransient(sp => (IReader<TEntity, int>)sp.GetRequiredService<TImplementation>());
		services.AddTransient(sp => (IReader<TEntity>)sp.GetRequiredService<TImplementation>());

		return services;
	}

	public static IServiceCollection AddDbContextPool<TContext>(
		this IServiceCollection services,
		string connectionStringName,
		Action<ConnectionStringInfo, DbContextOptionsBuilder> optionsAction,
		int poolSize = 1024)
		where TContext : DbContext
	{
		Ensure.IsNotNull(services, nameof(services));
		Ensure.IsNotNullOrEmpty(connectionStringName, nameof(connectionStringName));
		Ensure.IsNotNull(optionsAction, nameof(optionsAction));

		services.AddDbContextPool<TContext>((sp, options) =>
		{
			var csp = sp.GetRequiredService<IConnectionStringProvider>();
			if (!csp.TryGetConnectionString(connectionStringName, out var connectionString))
			{
				connectionString = csp.GetConnectionString();
			}

			optionsAction(connectionString!, options);

		}, poolSize);

		return services;
	}
}
