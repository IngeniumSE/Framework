namespace Ingenium.Platform.Security;

using static PermissionId;

public static class SecurityPermissions
{
	#region Users
	public static readonly Permission EditUsers = new(new("security.users:edit"), "Security: Edit Users", "Allows the ability to create or update users");
	public static readonly Permission DeleteUsers = new(new("security.users:delete"), "Security: Delete Users", "Allows the ability to delete users");
	public static readonly Permission ReadUsers = new(new("security.users:read"), "Security: Read Users", "Allows the ability to read users");


	public static readonly PermissionConvention UserManagement = new(new("security.users"), "Security: Users", "Allows management of users", Set(EditUsers.Id, DeleteUsers.Id, ReadUsers.Id));
	#endregion
}