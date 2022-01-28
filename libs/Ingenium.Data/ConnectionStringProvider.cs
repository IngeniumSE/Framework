namespace Ingenium.Data;

/// <summary>
/// Defines the required contract for implementing a connection string provider.
/// </summary>
public interface IConnectionStringProvider
{
	/// <summary>
	/// Attempts to get the connection string with the given name.
	/// </summary>
	/// <param name="name">The connection string name.</param>
	/// <param name="connectionString">The connection string.</param>
	/// <returns>True if the connection string could be read, otherwise false..</returns>
	bool TryGetConnectionString(string name, out ConnectionStringInfo? connectionString);
}

/// <summary>
/// Resolves connection strings from the configuration system.
/// </summary>
public class ConnectionStringProvider : IConnectionStringProvider
{
	readonly Dictionary<string, ConnectionStringInfo> _connectionStrings;

	public ConnectionStringProvider(DataOptions options)
	{
		_connectionStrings = ResolveConnectionStrings(options);
	}

	/// <inheritdoc />
	public bool TryGetConnectionString(string name, out ConnectionStringInfo? connectionString)
		=> _connectionStrings.TryGetValue(name, out connectionString);

	Dictionary<string, ConnectionStringInfo> ResolveConnectionStrings(DataOptions options)
	{
		Ensure.IsNotNull(options, nameof(options));

		var defaultProviderId = new ProviderId(options.DefaultProviderId);

		Dictionary<string, ConnectionStringInfo> connectionStrings = new(StringComparer.OrdinalIgnoreCase);
		if (options.ConnectionStrings is { Count: >0 })
		{
			foreach (var (name, connectionString) in options.ConnectionStrings)
			{
				var providerId = connectionString.ProviderId is { Length: > 0 }
					? new ProviderId(connectionString.ProviderId)
					: defaultProviderId;

				var info = new ConnectionStringInfo(
					name,
					connectionString.ConnectionString,
					providerId);

				connectionStrings.Add(name, info);
			}
		}

		return connectionStrings;
	}
}

/// <summary>
/// Provides extensions for the <see cref="IConnectionStringProvider"/> type.
/// </summary>
public static class ConnectionStringProviderExtensions
{
	/// <summary>
	/// Gets the connection string with the given name.
	/// </summary>
	/// <param name="name">The connection string name.</param>
	/// <returns>The connection string.</returns>
	public static ConnectionStringInfo GetConnectionString(this IConnectionStringProvider connectionStringProvider, string name = "default")
	{
		Ensure.IsNotNull(connectionStringProvider, nameof(connectionStringProvider));

		if (!connectionStringProvider.TryGetConnectionString(name, out var connectionString))
		{
			throw new InvalidOperationException($"The connection string '{name}' is not configured.");
		}

		return connectionString!;
	}
}