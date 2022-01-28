using System.Transactions;

using Microsoft.Extensions.Options;

namespace Ingenium.Data;

/// <summary>
/// Defines the required contract for implementing a transaction lifetime.
/// </summary>
public interface ITransactionLifetime : IDisposable
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

/// <summary>
/// Defines the required contract for implementing a transaction manager.
/// </summary>
public interface ITransactionManager
{
	/// <summary>
	/// Begins a transaction.
	/// </summary>
	/// <param name="isolationLevel">The transaction isolation level.</param>
	/// <param name="timeout">The transaction timeout.</param>
	/// <returns>The transaction lifetime</returns>
	ITransactionLifetime BeginTransaction(
			IsolationLevel? isolationLevel = default,
			TimeSpan? timeout = default);
}

/// <summary>
/// Provides services for managing transactions.
/// </summary>
public class TransactionManager : ITransactionManager
{
	private readonly DataOptions _options;

	/// <summary>
	/// Initialises a new instance of <see cref="TransactionManager"/>
	/// </summary>
	/// <param name="options">The data module options.</param>
	public TransactionManager(DataOptions options)
	{
		_options = Ensure.IsNotNull(options, nameof(options));
	}

	/// <inheritdoc />
	public ITransactionLifetime BeginTransaction(
			IsolationLevel? isolationLevel = default,
			TimeSpan? timeout = default)
			=> new MSTransactionLifetime(
					isolationLevel.GetValueOrDefault(_options.DefaultTransactionIsolationLevel),
					timeout.GetValueOrDefault(_options.DefaultTransactionTimeout)
			);
}

class MSTransactionLifetime : Disposable, ITransactionLifetime
{
	readonly TransactionScope _scope;

	public MSTransactionLifetime(IsolationLevel isolationLevel, TimeSpan timeout)
	{
		var options = new TransactionOptions
		{
			IsolationLevel = isolationLevel,
			Timeout = timeout
		};

		_scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled);
	}

	public Task CommitAsync(CancellationToken cancellationToken = default)
	{
		_scope.Complete();

		return Task.CompletedTask;
	}

	public Task RollbackAsync(CancellationToken cancellationToken = default)
	{
		_scope.Dispose();

		return Task.CompletedTask;
	}

	protected override void DisposeExplicit()
	{
		_scope.Dispose();
	}
}