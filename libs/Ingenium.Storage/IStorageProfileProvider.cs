namespace Ingenium.Storage;

/// <summary>
/// Defines the required contract for implementing a storage profile provider.
/// </summary>
public interface IStorageProfileProvider
{
	/// <summary>
	/// Attempts to return storage profile options for the given storage profile ID.
	/// </summary>
	/// <param name="profileId">The storage profile ID.</param>
	/// <param name="storageOptions">The storage options.</param>
	/// <param name="options">The storage options.</param>
	/// <returns>True if the provider returned options, otherwise false.</returns>
	bool TryGetStorageProfileOptions(StorageProfileId profileId, StorageOptions storageOptions, out StorageProfileOptions? options);
}
