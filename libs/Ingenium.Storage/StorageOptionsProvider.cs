using System.Collections.Concurrent;

using FluentValidation;

using Ingenium.Tenants;

using Microsoft.Extensions.Configuration;

namespace Ingenium.Storage;

/// <summary>
/// Defines the required contract for implementing a storage options provider.
/// </summary>
public interface IStorageOptionsProvider
{
	/// <summary>
	/// Gets the validated storage profile options.
	/// </summary>
	/// <typeparam name="TOptions">The storage profile options.</typeparam>
	/// <typeparam name="TValidator">The validator.</typeparam>
	/// <param name="profileId">The storage profile ID.</param>
	/// <param name="tenantId">The tenant ID.</param>
	/// <returns>The options.</returns>
	TOptions? GetStorageProfileOptions<TOptions, TValidator>(StorageProfileId profileId, TenantId tenantId)
		where TOptions : StorageProfileOptions, new()
		where TValidator : IValidator<TOptions>, new();
}

/// <summary>
/// Resolves instances of storage profile options.
/// </summary>
public class StorageOptionsProvider : IStorageOptionsProvider
{
	readonly IConfiguration _configuration;
	readonly IEnumerable<IStorageProfileProvider> _profileProviders;
	readonly StorageOptions _storageOptions;
	readonly ConcurrentDictionary<TenantScopedId<StorageProfileId>, StorageProfileOptions?> _profiles
		= new ConcurrentDictionary<TenantScopedId<StorageProfileId>, StorageProfileOptions?>(TenantScopedId<StorageProfileId>.Comparer);

	public StorageOptionsProvider(
		IConfiguration configuration, 
		IEnumerable<IStorageProfileProvider> profileProviders,
		StorageOptions storageOptions)
	{
		_configuration = Ensure.IsNotNull(configuration, nameof(configuration));
		_profileProviders = Ensure.IsNotNull(profileProviders, nameof(profileProviders));
		_storageOptions = Ensure.IsNotNull(storageOptions, nameof(storageOptions));
	}

	/// <inheritdoc />
	public TOptions? GetStorageProfileOptions<TOptions, TValidator>(StorageProfileId profileId, TenantId tenantId)
		where TOptions : StorageProfileOptions, new()
		where TValidator : IValidator<TOptions>, new()
	{
		TOptions? GetStorageProfileOptionsCore(TenantId? tenantId = default)
		{
			string key = $"{StorageOptions.ProfilesConfigurationSectionKey}:{profileId.Value}";
			if (tenantId.HasValue)
			{
				key = $"{key}:Tenants:{tenantId.Value}";
			}
			var section = _configuration.GetSection(key);

			TOptions? options = default;
			if (!section.Exists())
			{
				// Try and resolve options from profile providers.
				foreach (var provider in _profileProviders)
				{
					if (provider.TryGetStorageProfileOptions(profileId, tenantId.GetValueOrDefault(TenantId.Empty), _storageOptions, out var resolved))
					{
						options = resolved as TOptions;
						break;
					}
				}
			} 
			else
			{
				section.Bind(options);
			}

			return options;
		}

		var validator = new TValidator();

		var optionsId = new TenantScopedId<StorageProfileId>(TenantId.Empty, profileId);

		var options = _profiles.GetOrAdd(
			optionsId,
			p => GetStorageProfileOptionsCore()) as TOptions;

		if (!tenantId.Equals(TenantId.Empty) && !tenantId.Equals(TenantId.Default))
		{
			var tenantOptionsId = new TenantScopedId<StorageProfileId>(tenantId, profileId);
			var tenantOptions = _profiles.GetOrAdd(
				tenantOptionsId,
				p => GetStorageProfileOptionsCore(tenantId)
			);
		}

		validator.ValidateAndThrow(options);

		return options;
	}
}
