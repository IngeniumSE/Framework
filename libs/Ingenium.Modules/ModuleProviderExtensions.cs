namespace Ingenium.Modules;

/// <summary>
/// Provides extensions for the <see cref="IModuleProvider"/> type.
/// </summary>
public static class ModuleProviderExtensions
{
	/// <summary>
	/// Executes the given action for each module of the given type.
	/// </summary>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <param name="provider">The module provider.</param>
	/// <param name="action">The action to execute.</param>
	public static void ForEachModule<TService>(this IModuleProvider provider, Action<IModule, TService> action)
	{
		Ensure.IsNotNull(provider, nameof(provider));
		Ensure.IsNotNull(action, nameof(action));

		foreach (var module in provider.Modules)
		{
			if (module is TService service)
			{
				action(module, service);
			}
		}
	}

	/// <summary>
	/// Executes the given action for each module of the given type.
	/// </summary>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <param name="provider">The module provider.</param>
	/// <param name="action">The action to execute.</param>
	public static void ForEachModule<TService>(this IModuleProvider provider, Action<TService> action)
		where TService : class
		=> ForEachModule<TService>(provider, (module, service) => Ensure.IsNotNull(action, nameof(action))(service));

	/// <summary>
	/// Enumerates the modules and returns those as instances of the required service.
	/// </summary>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <param name="provider">The module provider.</param>
	/// <returns>The set of services.</returns>
	public static IEnumerable<TService> EnumerateModules<TService>(this IModuleProvider provider)
	{
		Ensure.IsNotNull(provider, nameof(provider));

		foreach (var module in provider.Modules)
		{
			if (module is TService service)
			{
				yield return service;
			}
		}
	}
}