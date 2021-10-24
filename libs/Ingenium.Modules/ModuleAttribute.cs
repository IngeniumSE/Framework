using System.Collections.ObjectModel;

namespace Ingenium.Modules;

/// <summary>
/// Adds descriptive properties to a module.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ModuleAttribute : Attribute
{
	/// <summary>
	/// Initialises a new instance of <see cref="ModuleAttribute"/>
	/// </summary>
	/// <param name="id">The module ID.</param>
	/// <param name="name">The module name.</param>
	/// <param name="description">The module description.</param>
	/// <param name="dependencies">The set of module dependnencies.</param>
	public ModuleAttribute(
		string id,
		string? name = default,
		string? description = default,
		params string[]? dependencies)
	{
		Id = new ModuleId(id);
		Name = name;
		Description = description;

		if (dependencies is { Length: > 0 })
		{
			Dependencies = new ReadOnlyCollection<ModuleId>(
				dependencies.Select(d => new ModuleId(d)).ToList()
			);
		}
	}

	/// <summary>
	/// Gets the module ID.
	/// </summary>
	public ModuleId Id { get; }

	/// <summary>
	/// Gets the module name.
	/// </summary>
	public string? Name { get; }

	/// <summary>
	/// Gets the module description.
	/// </summary>
	public string? Description { get; }

	/// <summary>
	/// Gets the set of dependencies.
	/// </summary>
	public IReadOnlyCollection<ModuleId>? Dependencies { get; }
}