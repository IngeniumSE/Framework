namespace Ingenium.Storage;

/// <summary>
/// Defines the required contract for implementing a storage provider.
/// </summary>
public interface IStorageProvider
{
	/// <summary>
	/// Puts the given file in storage.
	/// </summary>
	/// <param name="request">The put request.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The URI of the file.</returns>
	ValueTask<Uri> StoreAsync(StoreRequest request, CancellationToken cancellationToken = default);		
}

/// <summary>
/// Represents a base implementation of a storage provider.
/// </summary>
public abstract class StorageProvider : IStorageProvider
{
	readonly IStorageOptionsProvider _optionsProvider;

	protected StorageProvider(IStorageOptionsProvider optionsProvider)
	{
		_optionsProvider = Ensure.IsNotNull(optionsProvider, nameof(optionsProvider));
	}

	/// <summary>
	/// Gets the options provider.
	/// </summary>
	public IStorageOptionsProvider OptionsProvider => _optionsProvider;

	/// <inheritdoc />
	public abstract ValueTask<Uri> StoreAsync(StoreRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a request to store a file.
/// </summary>
/// <param name="ProfileId">The storage profile ID.</param>
/// <param name="RelativePath">The relative path.</param>
/// <param name="Source">The source stream.</param>
/// <param name="ConflictAction">The conflict action.</param>
public record StoreRequest(
	StorageProfileId ProfileId,
	string RelativePath,
	Stream Source,
	StoreConflictAction ConflictAction);