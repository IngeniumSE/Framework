namespace Ingenium.Modules;

/// <summary>
/// Represents a context used to initialize a module.
/// </summary>
/// <param name="ApplicationServices">The application service provider.</param>
public record ModuleInitializationContext(
	IServiceProvider ApplicationServices);