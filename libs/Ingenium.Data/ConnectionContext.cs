using System.Data;

namespace Ingenium.Data;

/// <summary>
/// Defines the required contract for implementing a connection context.
/// </summary>
public interface IConnectionContext : IDisposable
{
	/// <summary>
	/// Gets the connection.
	/// </summary>
	IDbConnection Connection { get; }

	/// <summary>
	/// Gets the connection string.
	/// </summary>
	ConnectionStringInfo ConnectionString { get; }
}

/// <summary>
/// Represents a connection context.
/// </summary>
public class ConnectionContext : Disposable, IConnectionContext
{
	/// <summary>
	/// Initialises a new instance of <see cref="ConnectionContext"/>
	/// </summary>
	/// <param name="connection">The connection.</param>
	/// <param name="connectionString">The connection string.</param>
	public ConnectionContext(
		IDbConnection connection,
		ConnectionStringInfo connectionString)
	{
		Connection = Ensure.IsNotNull(connection, nameof(connection));
		ConnectionString = Ensure.IsNotNull(connectionString, nameof(connectionString));
	}

	/// <inheritdoc />
	public IDbConnection Connection { get; }
	/// <inheritdoc />
	public ConnectionStringInfo ConnectionString { get; }

	/// <inheritdoc />
	protected override void DisposeExplicit()
		=> Connection.Dispose();
}