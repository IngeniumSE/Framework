using FluentValidation;

namespace Ingenium.Storage.FileSystem;

/// <summary>
/// Represents options for configuring a file system storage provider.
/// </summary>
public class FileSystemStorageProfileOptions : StorageProfileOptions
{
	public FileSystemStorageProfileOptions()
	{
		ProviderId = new("Storage.FileSystem");
	}

	/// <summary>
	/// Gets the root path.
	/// </summary>
	public string RootPath { get; set; } = default!;
}

/// <summary>
/// Validates instances of <see cref="FileSystemStorageProfileOptions"/>
/// </summary>
public class FileSystemStorageProfileOptionsValidator : StorageProfileOptionsValidator<FileSystemStorageProfileOptions>
{
	public FileSystemStorageProfileOptionsValidator()
	{
		RuleFor(o => o.RootPath).NotEmpty()
			.Custom((path, context) =>
			{
				if(!Path.IsPathRooted(path))
				{
					path = Path.GetFullPath(path);
				}

				if (!Directory.Exists(path))
				{
					context.AddFailure($"The root path '{path}' specified in the storage options does not exist.");
				}
			});
	}
}
