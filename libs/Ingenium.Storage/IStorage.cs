namespace Ingenium.Storage;

/// <summary>
/// Defines the required contract for implementing storage.
/// </summary>
public interface IStorage
{
	/// <summary>
	/// Puts the given file in storage.
	/// </summary>
	/// <param name="relativePath">The relative path.</param>
	/// <param name="source">The source file content.</param>
	/// <param name="conflictAction">Defines the behaviour to take when an existing file already exists at the given path.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The URI of the newly stored file.</returns>
	ValueTask<Uri> PutAsync(
		string relativePath,
		Stream source,
		StoreConflictAction conflictAction = StoreConflictAction.Throw,
		CancellationToken cancellationToken = default);
}

/// <summary>
/// Provides
/// </summary>
public class Storage : IStorage
{
	readonly IStorageProvider _provider;
	readonly StorageProfileId _profileId;

	public Storage(IStorageProvider provider, StorageProfileId profileId)
	{
		_provider = Ensure.IsNotNull(provider, nameof(provider));
		_profileId = profileId;
	}

	/// <inhertidoc />
	public async ValueTask<Uri> PutAsync(
		string relativePath, 
		Stream source,
		StoreConflictAction conflictAction = StoreConflictAction.Throw,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(relativePath, nameof(relativePath));
		Ensure.IsNotNull(source, nameof(source));

		var request = new StoreRequest(_profileId, relativePath, source, conflictAction);

		return await _provider.StoreAsync(request, cancellationToken);
	}
}

/// <summary>
/// Defines the behaviour to take when an existing file already exists.
/// </summary>
public enum StoreConflictAction
{
	Throw,
	Replace,
	Append
}
