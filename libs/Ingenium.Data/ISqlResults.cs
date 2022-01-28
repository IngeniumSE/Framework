namespace Ingenium.Data;

/// <summary>
/// Defines the required contract for implementing SQL results.
/// </summary>
public interface ISqlResults
{
	/// <summary>
	/// Reads all available records of the given type.
	/// </summary>
	/// <typeparam name="T">The record type.</typeparam>
	/// <returns>The set of results.</returns>
	Task<T[]> ReadAllAsync<T>();

	/// <summary>
	/// Reads the first record from the result set.
	/// Will throw a <see cref="InvalidOperationException"/> if no items exist in the set.
	/// </summary>
	/// <typeparam name="T">The record type.</typeparam>
	/// <returns>The first result.</returns>
	Task<T> ReadFirstAsync<T>();

	/// <summary>
	/// Reads the first record from the result set.
	/// </summary>
	/// <typeparam name="T">The record type.</typeparam>
	/// <returns>The first result.</returns>
	Task<T?> ReadFirstOrDefaultAsync<T>();

	/// <summary>
	/// Reads the only record from the result set.
	/// Will throw a <see cref="InvalidOperationException"/> if the numbers of items is not exactly 1.
	/// </summary>
	/// <typeparam name="T">The record type.</typeparam>
	/// <returns>The only result.</returns>
	Task<T> ReadSingleAsync<T>();

	/// <summary>
	/// Reads the only record from the result set.
	/// </summary>
	/// <typeparam name="T">The record type.</typeparam>
	/// <returns>The only result.</returns>
	Task<T?> ReadSingleOrDefaultAsync<T>();
}