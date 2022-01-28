using ConsoleSample;

using Ingenium.Hosting;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await new HostBuilder()
	.UseDiscoveredModules()
	.ConfigureLogging((ctx, lb) =>
	{
		lb.AddConfiguration(ctx.Configuration.GetSection("Logging"));
		lb.AddConsole();
	})
	.RunAppAsync<SampleApp>();