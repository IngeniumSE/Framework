namespace Ingenium.Storage;

/// <summary>
/// Defines the required contract for implementing a storage factory.
/// </summary>
public interface IStorageFactory
{
	/// <summary>
	/// Creates the storage instance for storing files.
	/// </summary>
	/// <param name="profileId">The storage profile ID.</param>
	/// <returns>The storage instance.</returns>
	IStorage CreateStorage(StorageProfileId profileId = default);
}

/// <summary>
/// Provides services for creating <see cref="IStorage"/> instances.
/// </summary>
public class StorageFactory : IStorageFactory
{
	readonly IProviderServiceFactory<IStorageProvider> _providerServiceFactory;
	readonly IEnumerable<IStorageProfileProvider> _providerProfiles;
	readonly StorageOptions _storageOptions;

	public StorageFactory(
		IProviderServiceFactory<IStorageProvider> providerServiceFactory,
		IEnumerable<IStorageProfileProvider> providerProfiles,
		StorageOptions storageOptions)
	{
		_providerServiceFactory = Ensure.IsNotNull(providerServiceFactory, nameof(providerServiceFactory));
		_providerProfiles = Ensure.IsNotNull(providerProfiles, nameof(providerProfiles));
		_storageOptions = Ensure.IsNotNull(storageOptions, nameof(storageOptions));
	}

	/// <inheritdoc />
	public IStorage CreateStorage(StorageProfileId profileId)
	{
		(profileId, var options) = GetStorageProfileOptions(profileId);
		if (options is null)
		{
			throw new ArgumentException($"Unable to resolve storage profile '{profileId}'");
		}
		var providerId = new ProviderId(options.ProviderId);

		var provider = _providerServiceFactory.GetProviderService(providerId);

		return new Storage(provider, profileId);
	}

	(StorageProfileId, StorageProfileOptions?) GetStorageProfileOptions(StorageProfileId profileId)
	{ 
		if (!profileId.HasValue && _storageOptions.DefaultProfileId is { Length: >0 })
		{
			profileId = new(_storageOptions.DefaultProfileId);
		}
		if (!profileId.HasValue)
		{
			throw new InvalidOperationException($"A default storage profile has not been configured.");
		}

		StorageProfileOptions? options = default;
		if (_storageOptions.Profiles is not { Count: >0 }
				|| !_storageOptions.Profiles.TryGetValue(profileId.Value, out options))
		{
			foreach (var profileProvider in _providerProfiles)
			{
				if (profileProvider.TryGetStorageProfileOptions(profileId, _storageOptions, out options))
				{
					return (profileId, options!);
				}
			}
		}

		return (profileId, options);
	}
}