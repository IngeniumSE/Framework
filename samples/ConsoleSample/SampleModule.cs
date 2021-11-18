using Ingenium.DependencyInjection;
using Ingenium.Modules;

using Microsoft.Extensions.DependencyInjection;

namespace ConsoleSample;

public class SampleModule : Module, IServicesBuilder
{
	public void AddServices(ServicesBuilderContext context, IServiceCollection services)
	{
	}
}
