using Ingenium.Configuration;
using Ingenium.DependencyInjection;

using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;

namespace Ingenium.Hosting;

/// <summary>
/// Provides extensions for the <see cref="IHostBuilder"/> type.
/// </summary>
public static class HostBuilderExtensions
{
	/// <summary>
	/// Configures the application to discover modules provided by the application.
	/// </summary>
	/// <param name="builder">The host builder.</param>
	/// <returns>The host builder.</returns>
	public static IHostBuilder UseDiscoveredModules(this IHostBuilder builder)
	{
		Ensure.IsNotNull(builder, nameof(builder));

		var context = DependencyContext.Default;
		var initializer = FrameworkInitializer.FromDependencyContext(context);

		builder.ConfigureAppConfiguration((context, builder) =>
		{
			initializer.AddConfiguration(
				new ConfigurationBuilderContext(
					context.HostingEnvironment,
					initializer.Modules,
					initializer.Parts),
				builder);
		});

		builder.ConfigureServices((context, services) =>
		{
			initializer.AddServices(
				new ServicesBuilderContext(
					context.Configuration,
					context.HostingEnvironment,
					initializer.Modules,
					initializer.Parts),
				services);
		});

		return builder;
	}
}