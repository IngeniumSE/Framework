namespace Ingenium.Data;

/// <summary>
/// Defines the required contract for implementing a connection context facory.
/// </summary>
public interface IConnectionContextFactory
{
	/// <summary>
	/// Creates a connection context.
	/// </summary>
	/// <param name="connectionStringName">The connection string name.</param>
	/// <returns>The connection context.</returns>
	IConnectionContext CreateConnectionContext(string connectionStringName = "Default");

	/// <summary>
	/// Creates a connection context.
	/// </summary>
	/// <param name="connectionString">The connection string</param>
	/// <returns>The connection context.</returns>
	IConnectionContext CreateConnectionContext(ConnectionStringInfo connectionString);
}