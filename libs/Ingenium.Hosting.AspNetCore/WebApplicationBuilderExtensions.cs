using Ingenium.Configuration;
using Ingenium.DependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyModel;

namespace Ingenium.Hosting;

/// <summary>
/// Provides extensions for the <see cref="WebApplicationBuilder"/>
/// </summary>
public static class WebApplicationBuilderExtensions
{
	/// <summary>
	/// Configures the web application to discover modules provided by the application.
	/// </summary>
	/// <param name="builder">The web application builder.</param>
	/// <returns>The web application builder.</returns>
	public static WebApplicationBuilder UseDiscoveredModules(this WebApplicationBuilder builder)
	{
		Ensure.IsNotNull(builder, nameof(builder));

		var context = DependencyContext.Default;
		var initializer = FrameworkInitializer.FromDependencyContext(context);

		builder.Host.ConfigureAppConfiguration((context, builder) =>
		{
			initializer.AddConfiguration(
				new ConfigurationBuilderContext(
					context.HostingEnvironment,
					initializer.Modules,
					initializer.Parts),
				builder);
		});

		builder.Host.ConfigureServices((context, services) =>
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
