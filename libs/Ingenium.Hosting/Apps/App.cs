using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ingenium.Hosting;

/// <summary>
/// Represents a runnable app.
/// </summary>
public abstract class App : IHostedService
{
	readonly IAppServices _services;

	/// <summary>
	/// Initialises a new instance of <see cref="App"/>
	/// </summary>
	/// <param name="services">The app services.</param>
	public App(IAppServices services)
	{
		_services = Ensure.IsNotNull(services, nameof(services));

		Logger = services.LoggerFactory.CreateLogger(GetType());
	}

	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Runs the application.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	protected abstract ValueTask RunAsync(CancellationToken cancellationToken);

	async Task IHostedService.StartAsync(CancellationToken cancellationToken)
	{
		try
		{
			await RunAsync(cancellationToken).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, ex.Message);

			Environment.ExitCode = -1;
		}

		_services.Lifetime.StopApplication();
	}

	Task IHostedService.StopAsync(CancellationToken cancellationToken)
		=> Task.CompletedTask;
}

/// <summary>
/// Provides extensions for the <see cref="IHostBuilder"/> type.
/// </summary>
public static class AppHostBuilderExtensions
{
	/// <summary>
	/// Runs the given app.
	/// </summary>
	/// <typeparam name="TApp">The app type.</typeparam>
	/// <param name="builder">The host builder.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The task instance.</returns>
	public static Task RunAppAsync<TApp>(
		this IHostBuilder builder,
		CancellationToken cancellationToken = default)
		where TApp : App
	{
		Ensure.IsNotNull(builder, nameof(builder));

		builder.ConfigureServices((context, services) =>
		{
			services.AddHostedService<TApp>();
		});

		return builder.RunConsoleAsync(cancellationToken);
	}
}