using Ingenium.Hosting;

using Microsoft.Extensions.Logging;

namespace ConsoleSample;

public class SampleApp : App
{
	public SampleApp(IAppServices services)
		: base(services) { }

	protected override ValueTask RunAsync(CancellationToken cancellationToken)
	{
		Logger.LogInformation("Example application.");

		return ValueTask.CompletedTask;
	}
}