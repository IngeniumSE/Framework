using Microsoft.Extensions.Configuration;

namespace Ingenium.Configuration;

/// <summary>
/// Defines the required contract for implementing a configuration extender.
/// </summary>
public interface IConfigurationExtender
{
	/// <summary>
	/// Adds services to the given service collection.
	/// </summary>
	/// <param name="context">The configuration builder context.</param>
	/// <param name="services">The configuration builder.</param>
	void AddConfiguration(ConfigurationBuilderContext context, IConfigurationBuilder builder);
}