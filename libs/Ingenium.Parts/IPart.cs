namespace Ingenium.Parts;

/// <summary>
/// Defines the required contract for implementing a part.
/// </summary>
public interface IPart
{
	/// <summary>
	/// Gets the name of the part.
	/// </summary>
	string Name { get; }
}