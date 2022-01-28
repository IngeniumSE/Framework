namespace Ingenium.Data;

/// <summary>
/// Defines the required contract for implementing a SQL factory.
/// </summary>
public interface ISqlFactory
{
	/// <summary>
	/// Creates a new <see cref="ISql"/> instance.
	/// </summary>
	/// <param name="connectionStringName">The connection string name.</param>
	/// <returns>The SQL context.</returns>
	ISql CreateSqlContext(string connectionStringName = "default");

	/// <summary>
	/// Creates a new <see cref="ISql"/> instance.
	/// </summary>
	/// <param name="connectionString">The connection string.</param>
	/// <returns>The SQL context.</returns>
	ISql CreateSqlContext(ConnectionStringInfo connectionString);
}