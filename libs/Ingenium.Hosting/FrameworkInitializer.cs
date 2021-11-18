using System.Collections.ObjectModel;

using Ingenium.Configuration;
using Ingenium.DependencyInjection;
using Ingenium.Modules;
using Ingenium.Parts;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Ingenium.Hosting;

/// <summary>
/// Provides services for low-level initialization of framework code.
/// </summary>
public class FrameworkInitializer
{
	/// <summary>
	/// Initialises a new instance of <see cref="FrameworkInitializer"/>
	/// </summary>
	/// <param name="parts">The part manager.</param>
	/// <param name="modules">The module provider.</param>
	public FrameworkInitializer(
		IPartManager parts,
		IModuleProvider modules)
	{
		Parts = Ensure.IsNotNull(parts, nameof(parts));
		Modules = Ensure.IsNotNull(modules, nameof(modules));	
	}

	/// <summary>
	/// Gets the module provider.
	/// </summary>
	public IModuleProvider Modules { get; }

	/// <summary>
	/// Gets the part manager.
	/// </summary>
	public IPartManager Parts { get; }

	/// <summary>
	/// Adds configuration provided by modules.
	/// </summary>
	/// <param name="context">The configuration builder context.</param>
	/// <param name="builder">The configuration builder.</param>
	public void AddConfiguration(ConfigurationBuilderContext context, IConfigurationBuilder builder)
	{
		Ensure.IsNotNull(context, nameof(context));
		Ensure.IsNotNull(builder, nameof(builder));

		foreach (var provider in context.Modules.EnumerateModules<IConfigurationExtender>())
		{
			provider.AddConfiguration(context, builder);
		}
	}

	/// <summary>
	/// Adds services provided by modules.
	/// </summary>
	/// <param name="context">The services builder context.</param>
	/// <param name="services">The services collection.</param>
	public void AddServices(ServicesBuilderContext context, IServiceCollection services)
	{
		Ensure.IsNotNull(context, nameof(context));
		Ensure.IsNotNull(services, nameof(services));

		// Add the module services.
		services.AddModuleServices(context);
	}

	/// <summary>
	/// Creates a <see cref="FrameworkInitializer"/> from the given dependency context.
	/// </summary>
	/// <param name="context">The dependency context.</param>
	/// <returns>The framework initializer.</returns>
	public static FrameworkInitializer FromDependencyContext(DependencyContext context)
	{
		Ensure.IsNotNull(context, nameof(context));

		var parts = CreatePartManager(context);
		var modules = CreateModuleProvider(parts);

		return new FrameworkInitializer(parts, modules);
	}

	static IPartManager CreatePartManager(DependencyContext context)
	{
		var assemblyProvider = new DependencyContextAssemblyProvider(context);
		var parts = new PartManager();

		foreach (var assembly in assemblyProvider.Assemblies)
		{
			parts.AddPart(new AssemblyPart(assembly));
		}

		return parts;
	}
	
	static IModuleProvider CreateModuleProvider(IPartManager parts)
	{
		parts.AddProvider(new ModulePartFeatureProvider());

		var feature = new ModulePartFeature();
		parts.PopulateFeature(feature);

		List<IModule> modules = new();
		foreach (var moduleType in feature.ModuleTypes)
		{
			var module = (IModule)Activator.CreateInstance(moduleType)!;
			modules.Add(module);
		}

		modules = modules
			.OrderByDependencies(m => m.Descriptor.Id, (module, id) => module.Descriptor.Dependencies)
			.ToList();

		return new ModuleProvider(new ReadOnlyCollection<IModule>(modules));
	}
}
