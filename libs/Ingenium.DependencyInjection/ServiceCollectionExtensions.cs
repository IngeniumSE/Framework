using FluentValidation;

using Ingenium.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

	/// <summary>
	/// Adds an injectable validator.
	/// </summary>
	/// <typeparam name="TModel">The model type.</typeparam>
	/// <typeparam name="TValidator">The validator type.</typeparam>
	/// <param name="services">The service collection.</param>
	/// <returns>The service colletion.</returns>
	public static IServiceCollection AddValidator<TModel, TValidator>(
		this IServiceCollection services)
		where TValidator : class, IValidator<TModel>
	{
		Ensure.IsNotNull(services, nameof(services));

		services.AddTransient<IValidator<TModel>, TValidator>();

		return services;
	}

	/// <summary>
	/// Configures options that are validated before resolution.
	/// </summary>
	/// <typeparam name="TOptions">The options type.</typeparam>
	/// <typeparam name="TValidator">The validator type.</typeparam>
	/// <param name="services">The services collection.</param>
	/// <param name="configuration">The configuration.</param>
	/// <returns>The service collection.</returns>
	public static IServiceCollection Configure<TOptions, TValidator>(
		this IServiceCollection services, 
		IConfiguration configuration)
		where TOptions : class
		where TValidator : class, IValidator<TOptions>
	{
		Ensure.IsNotNull(services, nameof(services));
		Ensure.IsNotNull(configuration, nameof(configuration));

		services.Configure<TOptions>(configuration);
		services.AddValidator<TOptions, TValidator>();

		services.AddSingleton(sp =>
		{
			var options = sp.GetRequiredService<IOptions<TOptions>>();
			var validator = sp.GetRequiredService<IValidator<TOptions>>();

			validator.ValidateAndThrow(options.Value);

			return options.Value;
		});

		return services;
	}

	/// <summary>
	/// Adds a singleton provider-mapped service.
	/// </summary>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <typeparam name="TImplementation">The implementation type.</typeparam>
	/// <param name="services">The services collection.</param>
	/// <returns>The services collection.</returns>
	public static IServiceCollection AddSingletonProviderService<TService, TImplementation>(this IServiceCollection services)
		where TImplementation : class, TService
		=> AddProviderService<TService, TImplementation>(services, ServiceLifetime.Singleton);

	/// <summary>
	/// Adds a scoped provider-mapped service.
	/// </summary>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <typeparam name="TImplementation">The implementation type.</typeparam>
	/// <param name="services">The services collection.</param>
	/// <returns>The services collection.</returns>
	public static IServiceCollection AddScopedProviderService<TService, TImplementation>(this IServiceCollection services)
		where TImplementation : class, TService
		=> AddProviderService<TService, TImplementation>(services, ServiceLifetime.Scoped);
	public static IServiceCollection AddTransientProviderService<TService, TImplementation>(this IServiceCollection services)
		where TImplementation : class, TService
		=> AddProviderService<TService, TImplementation>(services, ServiceLifetime.Transient);

	/// <summary>
	/// Adds a transient provider-mapped service.
	/// </summary>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <typeparam name="TImplementation">The implementation type.</typeparam>
	/// <param name="services">The services collection.</param>
	/// <returns>The services collection.</returns>

	static IServiceCollection AddProviderService<TService, TImplementation>(
		this IServiceCollection services, ServiceLifetime lifetime)
		where TImplementation : class, TService
	{
		Ensure.IsNotNull(services, nameof(services));

		var key = Provider<TService>.ForType(typeof(TImplementation));

		services.Add(new ServiceDescriptor(typeof(TImplementation), typeof(TImplementation), lifetime));
		services.Add(new ServiceDescriptor(typeof(Provider<TService>), sp => key, lifetime));

		return services;
	}
}
