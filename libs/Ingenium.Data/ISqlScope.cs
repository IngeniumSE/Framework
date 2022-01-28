using System.Transactions;

using Ingenium.Data;

/// <summary>
/// Defines the required contract for implementing a SQL context scope.
/// </summary>
public interface ISqlScope : IDisposable
{
	/// <summary>
	/// Gets the connection context associated with this scope.
	/// </summary>
	/// <returns>The connection context.</returns>
	IConnectionContext GetConnectionContext();

	/// <summary>
	/// Exectutes the given command, without returning the result.
	/// </summary>
	/// <param name="command">The command.</param>
	/// <param name="settings">[Optional] The command settings.</param>
	/// <returns>The task instance.</returns>
	Task ExecuteAsync(string command, CommandSettings settings = default);

	/// <summary>
	/// Reads all results from the given command.
	/// </summary>
	/// <typeparam name="T">The scalar type.</typeparam>
	/// <param name="command">The command.</param>
	/// <param name="settings">[Optional] The command settings.</param>
	/// <returns>The task instance who's result will be the set of records.</returns>
	Task<T[]> ReadAllAsync<T>(string command, CommandSettings settings = default);

	/// <summary>
	/// Reads all results from the given command and maps them with the provided map function
	/// </summary>
	/// <typeparam name="T">The type to be mapped to.</typeparam>
	/// <param name="command">The command.</param>
	/// <param name="mapAsync">The function that asynchronously maps the results to the return type.</param>
	/// <param name="settings">[Optional] The command settings.</param>
	/// <returns>The task instance who's result will be the set of mapped records.</returns>
	Task<T[]> ReadAllMappedAsync<T>(string command, Func<ISqlResults, Task<T[]>> mapAsync, CommandSettings settings = default);

	/// <summary>
	/// Reads the first record from the result of the given command, throwing an exception if there are no records.
	/// </summary>
	/// <typeparam name="T">The scalar type.</typeparam>
	/// <param name="command">The command.</param>
	/// <param name="settings">[Optional] The command settings.</param>
	/// <returns>The task instance who's result will be the first record.</returns>
	Task<T> ReadFirstAsync<T>(string command, CommandSettings settings = default);

	/// <summary>
	/// Reads the first record from the result of the given command.
	/// </summary>
	/// <typeparam name="T">The scalar type.</typeparam>
	/// <param name="command">The command.</param>
	/// <param name="settings">[Optional] The command settings.</param>
	/// <returns>The task instance who's result will be the first record.</returns>
	Task<T> ReadFirstOrDefaultAsync<T>(string command, CommandSettings settings = default);

	/// <summary>
	/// Reads the scalar value from the result of the given command.
	/// </summary>
	/// <typeparam name="T">The scalar type.</typeparam>
	/// <param name="command">The command.</param>
	/// <param name="settings">[Optional] The command settings.</param>
	/// <returns>The task instance who's result will be the scalar value.</returns>
	Task<T> ReadScalarAsync<T>(string command, CommandSettings settings = default);

	/// <summary>
	/// Reads one record from the result of the given command, throwing an exception if there is not exactly
	/// 1 record.
	/// </summary>
	/// <typeparam name="T">The scalar type.</typeparam>
	/// <param name="command">The command.</param>
	/// <param name="settings">[Optional] The command settings.</param>
	/// <returns>The task instance who's result will be the single record.</returns>
	Task<T> ReadSingleAsync<T>(string command, CommandSettings settings = default);

	/// <summary>
	/// Reads all results from the given command and maps them to a single record with the provided map function
	/// </summary>
	/// <typeparam name="T">The type to be mapped to.</typeparam>
	/// <param name="command">The command.</param>
	/// <param name="mapAsync">The function that asynchronously maps the results to the return type.</param>
	/// <param name="settings">[Optional] The command settings.</param>
	/// <returns>The task instance who's result will be the mapped record.</returns>
	Task<T> ReadSingleMappedAsync<T>(string command, Func<ISqlResults, Task<T>> mapAsync, CommandSettings settings = default);

	/// <summary>
	/// Reads one record from the result of the given command.
	/// </summary>
	/// <typeparam name="T">The scalar type.</typeparam>
	/// <param name="command">The command.</param>
	/// <param name="settings">[Optional] The command settings.</param>
	/// <returns>The task instance who's result will be the single record.</returns>
	Task<T> ReadSingleOrDefaultAsync<T>(string command, CommandSettings settings = default);

	/// <summary>
	/// Streams all of the results from the source.
	/// </summary>
	/// <typeparam name="T">The record type.</typeparam>
	/// <param name="command">The command.</param>
	/// <param name="settings">[Optional] The command settings.</param>
	/// <returns>The async enumerable.</returns>
	IAsyncEnumerable<T> StreamAllAsync<T>(string command, CommandSettings settings = default);
}

/// <summary>
/// Defines the required contract for implementing a SQL context.
/// </summary>
public interface ISql : ISqlScope
{
	/// <summary>
	/// Begins a SQL scope which manages a SQL connection accross multiple calls.
	/// </summary>
	/// <returns></returns>
	ISqlScope BeginScope();

	/// <summary>
	/// Begins a Sql scope which manages a Sql connection across multiple calls, backed by a transaction.
	/// </summary>
	/// <param name="isolationLevel">The transaction isolation level.</param>
	/// <param name="timeout">The transaction timeout</param>
	/// <returns>The Sql scope.</returns>
	ISqlTransactionScope BeginTransactionScope(
			IsolationLevel? isolationLevel = default,
			TimeSpan? timeout = default);
}

/// <summary>
/// Defines the required contract for implementing a SQL scoped backed by an active transaction.
/// </summary>
public interface ISqlTransactionScope : ISqlScope
{
	/// <summary>
	/// Commits the active transaction.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The task instance.</returns>
	Task CommitAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Rolls back the active transaction.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The task instance.</returns>
	Task RollbackAsync(CancellationToken cancellationToken = default);
}