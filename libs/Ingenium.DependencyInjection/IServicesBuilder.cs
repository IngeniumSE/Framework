using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.DependencyInjection;

/// <summary>
/// Defines the required contract for implementing a services builder.
/// </summary>
public interface IServicesBuilder
{
	/// <summary>
	/// Adds services to the given service collection.
	/// </summary>
	/// <param name="context">The services builder context.</param>
	/// <param name="services">The service collection..</param>
	void AddServices(ServicesBuilderContext context, IServiceCollection services);
}