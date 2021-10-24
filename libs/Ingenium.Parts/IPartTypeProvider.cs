using System.Reflection;

namespace Ingenium.Parts;

/// <summary>
/// Defines the required contract for implementing a part.
/// </summary>
public interface IPartTypeProvider
{
	/// <summary>
	/// Gets the set of types.
	/// </summary>
	IEnumerable<TypeInfo> Types { get; }
}