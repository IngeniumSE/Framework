using Ingenium.Modules;
using Ingenium.Parts;

using Microsoft.Extensions.Hosting;

namespace Ingenium.Configuration;

/// <summary>
/// Provides a context for building configuration.
/// </summary>
/// <param name="Environment">The environment.</param>
/// <param name="Modules">The module provider.</param>
/// <param name="Parts">The parts.</param>
public record ConfigurationBuilderContext(
	IHostEnvironment Environment,
	IModuleProvider Modules,
	IPartManager Parts);
