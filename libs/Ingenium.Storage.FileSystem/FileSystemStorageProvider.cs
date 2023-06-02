namespace Ingenium.Storage.FileSystem;

/// <summary>
/// Provides storage services using the local file system.
/// </summary>
public class FileSystemStorageProvider : StorageProvider
{
	public FileSystemStorageProvider(IStorageOptionsProvider optionsProvider)
		: base(optionsProvider) { }

	/// <inheritdoc />
	public override async ValueTask<Uri> StoreAsync(StoreRequest request, CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNull(request, nameof(request));

		var options = OptionsProvider
			.GetStorageProfileOptions<FileSystemStorageProfileOptions, FileSystemStorageProfileOptionsValidator>(
				request.ProfileId, request.TenantId)!;

		string rootPath = options.RootPath;
		if (!Path.IsPathRooted(rootPath))
		{
			rootPath = Path.GetFullPath(rootPath);
		}

		string fullPath = Path.Combine(rootPath, request.RelativePath);
		string directoryPath = Path.GetDirectoryName(fullPath)!;
		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}

		var fileMode = request.ConflictAction switch
		{
			StoreConflictAction.Throw => FileMode.CreateNew,
			StoreConflictAction.Replace => FileMode.Create,
			_ => FileMode.Append
		};

		using var file = new FileStream(fullPath, fileMode, FileAccess.Write, FileShare.None);
		// Reset the stream position.
		request.Source.Position = 0;

		await request.Source.CopyToAsync(file);
		await file.FlushAsync();

		return new Uri(fullPath);
	}
}
