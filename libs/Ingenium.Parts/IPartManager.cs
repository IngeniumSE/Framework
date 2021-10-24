namespace Ingenium.Parts;

/// <summary>
/// Defines the required contract for implementing a part.
/// </summary>
public interface IPartManager
{
	/// <summary>
	/// Adds a part.
	/// </summary>
	/// <param name="part">The part to add.</param>
	void AddPart(IPart part);

	/// <summary>
	/// Adds a part feature provider.
	/// </summary>
	/// <param name="provider">The part feature provider.</param>
	void AddProvider(IPartFeatureProvider provider);

	/// <summary>
	/// Populates the given feature.
	/// </summary>
	/// <typeparam name="TPartFeature">The part feature.</typeparam>
	/// <param name="feature">The feature instance.</param>
	void PopulateFeature<TPartFeature>(TPartFeature feature)
		where TPartFeature : class;
}

/// <summary>
/// Provides servivces for managing parts.
/// </summary>
public class PartManager : IPartManager
{
	readonly List<IPartFeatureProvider> _providers =new();
	readonly List<IPart> _parts = new();

	/// <inheritdoc />
	public void AddPart(IPart part)
	{
		Ensure.IsNotNull(part, nameof(part));

		_parts.Add(part);
	}

	/// <inheritdoc />
	public void AddProvider(IPartFeatureProvider provider)
	{
		Ensure.IsNotNull(provider, nameof(provider));

		_providers.Add(provider);
	}

	/// <inheritdoc />
	public void PopulateFeature<TPartFeature>(TPartFeature feature) where TPartFeature : class
	{
		Ensure.IsNotNull(feature, nameof(feature));

		foreach (var provider in _providers)
		{
			if (provider is IPartFeatureProvider<TPartFeature> partFeatureProvider)
			{
				partFeatureProvider.PopulateFeature(_parts, feature);
			}
		}
	}
}