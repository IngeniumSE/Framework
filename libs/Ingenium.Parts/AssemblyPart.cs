using System.Reflection;

namespace Ingenium.Parts;

/// <summary>
/// An assembly-backed part.
/// </summary>
public sealed class AssemblyPart: IPart, IPartTypeProvider
{
	/// <summary>
	/// Initialises a new instance of <see cref="AssemblyPart"/>.
	/// </summary>
	/// <param name="assembly">The assembly.</param>
	public AssemblyPart(Assembly assembly)
	{
		Assembly = Ensure.IsNotNull(assembly, nameof(assembly));
	}

	/// <summary>
	/// Gets the assembly.
	/// </summary>
	public Assembly Assembly { get; }

	/// <inheritdoc />
	public string Name => Assembly.GetName().Name!;

	/// <inheritdoc />
	public IEnumerable<TypeInfo> Types => Assembly.DefinedTypes;
}