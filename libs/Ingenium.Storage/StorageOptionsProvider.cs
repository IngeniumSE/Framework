using System.Collections.Concurrent;

using FluentValidation;

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
	/// <returns>The options.</returns>
	TOptions? GetStorageProfileOptions<TOptions, TValidator>(StorageProfileId profileId)
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
	readonly ConcurrentDictionary<StorageProfileId, StorageProfileOptions?> _profiles
		= new ConcurrentDictionary<StorageProfileId, StorageProfileOptions?>(StorageProfileId.Comparer);

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
	public TOptions? GetStorageProfileOptions<TOptions, TValidator>(StorageProfileId profileId)
		where TOptions : StorageProfileOptions, new()
		where TValidator : IValidator<TOptions>, new()
	{
		TOptions? GetStorageProfileOptionsCore()
		{
			string key = $"{StorageOptions.ProfilesConfigurationSectionKey}:{profileId.Value}";
			var section = _configuration.GetSection(key);

			TOptions? options = default;
			var validator = new TValidator();
			if (!section.Exists())
			{
				// Try and resolve options from profile providers.
				foreach (var provider in _profileProviders)
				{
					if (provider.TryGetStorageProfileOptions(profileId, _storageOptions, out var resolved))
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

			validator.ValidateAndThrow(options);

			return options;
		}

		var options = _profiles.GetOrAdd(
			profileId,
			p => GetStorageProfileOptionsCore()) as TOptions;

		return options;
	}
}
