namespace Ingenium.Parts;

/// <summary>
/// Defines the required contract for implementing a part feature provider.
/// </summary>
public interface IPartFeatureProvider
{
}

/// <summary>
/// Defines the required contract for implementing a part feature provider.
/// </summary>
/// <typeparam name="TPartFeature">The part feature type.</typeparam>
public interface IPartFeatureProvider<TPartFeature> : IPartFeatureProvider
	where TPartFeature : class
{
	/// <summary>
	/// Populates the given feature.
	/// </summary>
	/// <param name="parts">The set of available parts.</param>
	/// <param name="feature">The part feature.</param>
	void PopulateFeature(IEnumerable<IPart> parts, TPartFeature feature);
}