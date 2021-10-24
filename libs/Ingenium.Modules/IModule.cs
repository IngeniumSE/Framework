using System.Reflection;

namespace Ingenium.Modules;

/// <summary>
/// Defines the required contract for implementing a module.
/// </summary>
public interface IModule
{
	/// <summary>
	/// Gets the module descriptor.
	/// </summary>
	ModuleDescriptor Descriptor { get; }

	/// <summary>
	/// Initializes the module.
	/// </summary>
	/// <param name="context">The initialization context.</param>
	void Initialize(ModuleInitializationContext context) { }
}

/// <summary>
/// Provides a base implementation of a module.
/// </summary>
public abstract class Module : IModule
{
	/// <summary>
	/// Initialises a new instance of <see cref="Module"/>
	/// </summary>
	protected Module()
	{
		Descriptor = GenerateDescriptor();
	}

	/// <summary>
	/// Initialises a new instance of <see cref="Module"/>
	/// </summary>
	/// <param name="descriptor">The module descriptor.</param>
	protected Module(ModuleDescriptor descriptor)
	{
		Descriptor = Ensure.IsNotNull(descriptor, nameof(descriptor));
	}

	/// <inheritdoc />
	public ModuleDescriptor Descriptor {  get; }

	ModuleDescriptor GenerateDescriptor()
	{
		var type = GetType();
		var attr = type.GetCustomAttribute<ModuleAttribute>(inherit: false);

		if (attr is null)
		{
			string id = type.Name.Replace("Module", "");

			return new ModuleDescriptor(
				new ModuleId(id)
			);
		}

		return new ModuleDescriptor(
			attr.Id,
			attr.Name,
			attr.Description,
			attr.Dependencies
		);
	}
}