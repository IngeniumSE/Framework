using Ingenium.Tenants;

namespace Ingenium.Storage.FileSystem;

/// <summary>
/// Provides implicit file system storage providers.
/// </summary>
public class FileSystemStorageProfileProvider : IStorageProfileProvider
{
	/// <inheritdoc />
	public bool TryGetStorageProfileOptions(
		StorageProfileId profileId, 
		TenantId tenantId,
		StorageOptions storageOptions, 
		out StorageProfileOptions? options)
	{
		Ensure.IsNotNull(storageOptions, nameof(storageOptions));

		if (storageOptions.ImplicitTempFileSystemProfile && WellKnownStorageProfiles.Temp.Equals(profileId))
		{
			options = CreateOptions(
				Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
			);
			return true;
		}

		if (storageOptions.ImplicitLocalFileSystemProfile && WellKnownStorageProfiles.Local.Equals(profileId))
		{
			string path = "./files";
			if (!tenantId.IsEmpty())
			{
				path = $"/{tenantId.Value}";
			}

			options = CreateOptions(path);
			return true;
		}

		options = default;
		return false;
	}

	FileSystemStorageProfileOptions CreateOptions(string rootPath)
	{
		if (!Path.IsPathRooted(rootPath))
		{
			rootPath = Path.GetFullPath(rootPath);
		}

		if (!Directory.Exists(rootPath))
		{
			Directory.CreateDirectory(rootPath);
		}

		var options = new FileSystemStorageProfileOptions
		{
			RootPath = rootPath
		};

		return options;
	}
}
