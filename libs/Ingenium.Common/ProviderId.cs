using Ingenium.CodeGeneration;

namespace Ingenium;

[GenerateId(typeof(string), caseInsensitive: true)]
public partial struct ProviderId { }

/// <summary>
/// Decorates a type, or an assembly as scoped to a specific provider.
/// </summary>
[
	AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Assembly,
		AllowMultiple = false,
		Inherited = false
	)
]
public class ProviderAttribute : Attribute
{
	/// <summary>
	/// Initialises a new instance of <see cref="ProviderAttribute"/>.
	/// </summary>
	/// <param name="providerId">The provider ID.</param>
	public ProviderAttribute(string providerId)
	{
		ProviderId = new(providerId);
	}

	/// <summary>
	/// Gets the provider ID.
	/// </summary>
	public ProviderId ProviderId { get; }
}

/// <summary>
/// Defines the required contract for implementing a provider factory.
/// </summary>
/// <typeparam name="TService">The service type.</typeparam>
public interface IProviderServiceFactory<TService>
{
	/// <summary>
	/// Gets the provider service for the given provider ID.
	/// </summary>
	/// <param name="providerId">The provider ID.</param>
	/// <returns>The service instance.</returns>
	TService GetProviderService(ProviderId providerId);
}