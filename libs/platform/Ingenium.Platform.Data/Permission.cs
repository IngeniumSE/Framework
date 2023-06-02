namespace Ingenium.Platform.Security;

/// <summary>
/// Represents a permission ID.
/// </summary>
[GenerateId(typeof(string), caseInsensitive: true)]
public partial struct PermissionId
{
	/// <summary>
	/// Creates a set of permissions
	/// </summary>
	/// <param name="perms">The permissions.</param>
	/// <returns>The set of permissions.</returns>
	public static PermissionId[] Set(params PermissionId[] perms)
		=> perms is { Length: > 0 } ? perms : Array.Empty<PermissionId>();
}

/// <summary>
/// Represents a permission.
/// </summary>
/// <param name="Id">The permission ID.</param>
/// <param name="Name">The permission name.</param>
/// <param name="Description">The permission description.</param>
public record Permission(
	PermissionId Id,
	string Name,
	string? Description = default)
{
	/// <summary>
	/// Creates a set of permissions.
	/// </summary>
	/// <param name="perms">The permissions.</param>
	/// <returns>The set of permissions.</returns>
	public static Permission[] Set(params Permission[] perms)
		=> perms is { Length: > 0 } ? perms : Array.Empty<Permission>();
}