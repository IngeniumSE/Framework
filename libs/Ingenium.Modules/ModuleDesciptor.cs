namespace Ingenium.Modules;

/// <summary>
/// Represents a module descriptor.
/// </summary>
/// <param name="Id">The module ID.</param>
/// <param name="Name">The module name.</param>
/// <param name="Description">The module description.</param>
/// <param name="Dependencies">The modules that this module has an explicit dependency on.</param>
public record ModuleDescriptor(
	ModuleId Id,
	string? Name = default,
	string? Description = default,
	IReadOnlyCollection<ModuleId>? Dependencies = default);