using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ingenium.Hosting;

/// <summary>
/// Defines the required contract for implementing app services.
/// </summary>
public interface IAppServices
{
	/// <summary>
	/// Gets the lifetime.
	/// </summary>
	IHostApplicationLifetime Lifetime { get; }

	/// <summary>
	/// Gets the logger factory.
	/// </summary>
	ILoggerFactory LoggerFactory { get; }
}

/// <summary>
/// Provides services for apps.
/// </summary>
/// <param name="Lifetime">The host application lifetime.</param>
/// <param name="LoggerFactory">The logger factory.</param>
public record AppServices(IHostApplicationLifetime Lifetime, ILoggerFactory LoggerFactory) : IAppServices;