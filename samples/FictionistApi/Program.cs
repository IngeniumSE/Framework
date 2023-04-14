using Auth0.AspNetCore.Authentication;

using Ingenium.Hosting;

class Program
{
	readonly IConfiguration _config;

	public Program(IConfiguration config)
	{
		_config = Ensure.IsNotNull(config, nameof(config));
	}

	public static Task Main()
	{
		var builder = new HostBuilder()
			.UseDiscoveredModules()
			.ConfigureLogging(b => b.AddConsole())
			.ConfigureWebHostDefaults(b => b
				.UseStartup<Program>()
				.UseKestrel(k =>
				{
					k.ListenLocalhost(5000, o => o.UseHttps());
				})
			)
			.Build();

		return builder.RunAsync();
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddSwaggerGen(o =>
		{
			
		});
		services.AddControllersWithViews();

		//services.AddAuth0WebAppAuthentication(o =>
		//{
		//	o.Domain = _config["OAuth:Domain"];
		//	o.ClientId = _config["OAuth:ClientId"];
		//	o.ClientSecret = _config["OAuth:ClientSecret"];
		//});
	}

	public void Configure(IApplicationBuilder app)
	{
		app.UseAuthentication();

		app.UseRouting();
		//app.UseAuthorization();
		app.UseEndpoints(b =>
		{
			b.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
		});
		app.UseSwagger();
		app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));
	}
}