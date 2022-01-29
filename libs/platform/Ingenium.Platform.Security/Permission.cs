using Ingenium.CodeGeneration;

namespace Ingenium.Platform.Security;

/// <summary>
/// Represents a permission ID.
/// </summary>
[GenerateId(typeof(string), caseInsensitive: true)]
public partial struct PermissionId { }

/// <summary>
/// Represents a permission.
/// </summary>
/// <param name="Id">The permission ID.</param>
/// <param name="Name">The permission name.</param>
/// <param name="Description">The permission description.</param>
public record Permission(
	PermissionId Id,
	string Name,
	string? Description = default);