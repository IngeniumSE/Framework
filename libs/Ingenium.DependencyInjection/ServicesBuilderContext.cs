using Ingenium.Modules;
using Ingenium.Parts;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Ingenium.DependencyInjection;

/// <summary>
/// Provides a context for building services.
/// </summary>
/// <param name="Configuration">The application configuration.</param>
/// <param name="Environment">The environment.</param>
/// <param name="Modules">The module provider.</param>
/// <param name="Parts">The parts.</param>
public record ServicesBuilderContext(
	IConfiguration Configuration,
	IHostEnvironment Environment,
	IModuleProvider Modules,
	IPartManager Parts);
