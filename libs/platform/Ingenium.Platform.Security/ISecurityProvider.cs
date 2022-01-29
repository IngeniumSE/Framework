namespace Ingenium.Platform.Security;

/// <summary>
/// Defines the required contract for implementing a security provider.
/// </summary>
public interface ISecurityProvider
{
	/// <summary>
	/// Gets the permissions provided by this provider.
	/// </summary>
	/// <returns>The set of permissions.</returns>
	IEnumerable<Permission> GetPermissions();

	/// <summary>
	/// Gets the permission conventions provided by this provider.
	/// </summary>
	/// <returns>The set of permission conventions.</returns>
	IEnumerable<PermissionConvention> GetPermissionConventions();
}
