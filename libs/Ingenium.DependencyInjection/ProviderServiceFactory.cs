using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.DependencyInjection;

/// <summary>
/// Provides resolution of provider services for named provider IDs.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
public class ProviderServiceFactory<TService> : IProviderServiceFactory<TService>
{
	readonly IServiceProvider _services;
	readonly IEnumerable<Provider<TService>> _providers;

	public ProviderServiceFactory(
		IServiceProvider services,
		IEnumerable<Provider<TService>> providers)
	{
		_services = Ensure.IsNotNull(services, nameof(services));
		_providers = Ensure.IsNotNull(providers, nameof(providers));
	}

	/// <inheritdoc />
	public TService GetProviderService(ProviderId providerId)
	{
		var provider = _providers.FirstOrDefault(p => p.ProviderId.Equals(providerId));
		if (provider is null)
		{
			throw new ArgumentException($"The provider '{providerId.Value}' does not provide service type '{typeof(TService).Name}'.");
		}

		return (TService)_services.GetRequiredService(provider.ImplementationType);
	}
}

/// <summary>
/// Represents the definition between a provider ID and an implementation type.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
/// <param name="ProviderId">The provider ID.</param>
/// <param name="ImplementationType">The implementation type.</param>
public record Provider<TService>(ProviderId ProviderId, Type ImplementationType)
{
	/// <summary>
	/// Resolves the provider ID for the given type.
	/// </summary>
	/// <param name="type">The implementation type.</param>
	/// <returns>The provider mapping.</returns>
	/// <exception cref="InvalidOperationException">If no provider ID could be resolved for the given type.</exception>
	public static Provider<TService> ForType(Type type)
	{
		Ensure.IsNotNull(type, nameof(type));

		var attr = type.GetCustomAttribute<ProviderAttribute>()
			?? type.Assembly.GetCustomAttribute<ProviderAttribute>();

		if (attr is null)
		{
			throw new InvalidOperationException($"No provider implementation '{type.Name}' does not define a provider ID.");
		}

		return new Provider<TService>(attr.ProviderId, type);
	}
}