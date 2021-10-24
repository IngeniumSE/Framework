namespace Ingenium.Modules;

/// <summary>
/// Defines the required contract for implementing a module provider.
/// </summary>
public interface IModuleProvider
{
	/// <summary>
	/// Gets the set of modules.
	/// </summary>
	IReadOnlyCollection<IModule> Modules { get; }
}

/// <summary>
/// Provides services for returning a set of modules.
/// </summary>
public class ModuleProvider : IModuleProvider
{
	/// <summary>
	/// Initialises a new instance of <see cref="ModuleProvider"/>
	/// </summary>
	/// <param name="modules">The set of modules.</param>
	public ModuleProvider(IReadOnlyCollection<IModule> modules)
	{
		Modules = Ensure.IsNotNull(modules, nameof(modules));
	}

	/// <inheritdoc />
	public IReadOnlyCollection<IModule> Modules { get; }
}