using Ingenium.Data;
using Ingenium.DependencyInjection;
using Ingenium.Modules;
using Ingenium.Platform.Data;
using Ingenium.Platform.Security.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.Platform.Security;

/// <summary>
/// Allows composition of platform security services into an application.
/// </summary>
[Module(
	id: "Platform.Security",
	name: "Platform.Security",
	description: "Provides platform security services for applications.")]
public class PlatformSecurityModule : Module, IServicesBuilder
{
	/// <inheritdoc />
	public void AddServices(ServicesBuilderContext context, IServiceCollection services)
	{
		Ensure.IsNotNull(context, nameof(context));
		Ensure.IsNotNull(services, nameof(services));

		services.AddDbContextPool<SecurityDbContext>(
			"Security", 
			(connectionString, options) =>
		{
			options.UseSqlServer(connectionString.ConnectionString);
		});

		services.AddEntityReader<IUserReader, UserReader, User, UserId>();
	}
}