using Ingenium.Platform.Security;

namespace Ingenium.Platform.Shell;

/// <summary>
/// Represents a menu ID.
/// </summary>
[GenerateId(typeof(string))]
public partial struct MenuId { }

/// <summary>
/// Represents a menu item ID.
/// </summary>
[GenerateId(typeof(string))]
public partial struct MenuItemId { }

/// <summary>
/// Represents a menu group ID.
/// </summary>
[GenerateId(typeof(string))]
public partial struct MenuGroupId { }

/// <summary>
/// Represents a menu item.
/// </summary>
/// <param name="id">The menu item ID.</param>
/// <param name="requiredPermission">The required permission for the menu item.</param>
public record MenuItem(MenuItemId id, PermissionId? requiredPermission = default);

/// <summary>
/// Represents a menu.
/// </summary>
/// <param name="id">The menu ID.</param>
/// <param name="items">The set of menu items.</param>
/// <param name="groupId">The menu group ID.</param>
public record Menu(MenuId id, MenuItem[] items, MenuGroupId? groupId = null);

/// <summary>
/// Defines the required contract for implementing a menu provider.
/// </summary>
public interface IMenuProvider
{
	/// <summary>
	/// Gets the set of menus for the given group ID.
	/// </summary>
	/// <param name="groupId">The menu group ID.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The set of menus.</returns>
	ValueTask<Menu[]> GetMenusAsync(MenuGroupId? groupId = default, CancellationToken cancellationToken = default);
}