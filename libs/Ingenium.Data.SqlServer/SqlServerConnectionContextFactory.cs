using Microsoft.Data.SqlClient;

namespace Ingenium.Data.SqlServer;

/// <summary>
/// Creates connection contexts for Sql Server connections.
/// </summary>
public class SqlServerConnectionContextFactory : IConnectionContextFactory
{
	readonly IConnectionStringProvider _connectionStrings;

	public SqlServerConnectionContextFactory(IConnectionStringProvider connectionStrings)
	{
		_connectionStrings = Ensure.IsNotNull(connectionStrings, nameof(connectionStrings));
	}

	/// <inheritdoc />
	public IConnectionContext CreateConnectionContext(string connectionStringName = "Default")
	{
		Ensure.IsNotNullOrEmpty(connectionStringName, nameof(connectionStringName));
	
		var connectionString = _connectionStrings.GetConnectionString(connectionStringName);

		return CreateConnectionContext(connectionString);
	}

	/// <inheritdoc />
	public IConnectionContext CreateConnectionContext(ConnectionStringInfo connectionString)
	{
		Ensure.IsNotNull(connectionString, nameof(connectionString));

		return new ConnectionContext(
			CreateConnection(connectionString.ConnectionString), 
			connectionString);
	}

	SqlConnection CreateConnection(string connectionString)
	{
		var connection = new SqlConnection(connectionString);

		return connection;
	}
}