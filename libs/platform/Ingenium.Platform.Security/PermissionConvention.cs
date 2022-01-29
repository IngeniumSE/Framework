using Ingenium.CodeGeneration;

namespace Ingenium.Platform.Security;

/// <summary>
/// Represents a permission convention ID.
/// </summary>
[GenerateId(typeof(string), caseInsensitive: true)]
public partial struct PermissionConventionId { }

/// <summary>
/// Represents a permission convention.
/// </summary>
/// <param name="Id">The permission convention ID.</param>
/// <param name="Name">The permission convention name.</param>
/// <param name="Description">The permission convention description.</param>
/// <param name="Permissions">The set of applied permissions.</param>
public record PermissionConvention(
	PermissionConventionId Id,
	string Name,
	string? Description = default,
	PermissionId[]? Permissions = default);