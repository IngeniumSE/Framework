using ConsoleSample;

using Ingenium.Hosting;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await new HostBuilder()
	.UseDiscoveredModules()
	.ConfigureLogging(lb =>
	{
		lb.AddConsole();
	})
	.RunAppAsync<SampleApp>();