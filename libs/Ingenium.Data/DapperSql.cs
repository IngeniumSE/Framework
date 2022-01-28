using System.Data.Common;

using Dapper;

using Ingenium.DependencyInjection;

using IsolationLevel = System.Transactions.IsolationLevel;

namespace Ingenium.Data;

/// <summary>
/// Provides an instance of <see cref="ISql"/> backed by Dapper.
/// </summary>
public class DapperSql : Disposable, ISql
{
	readonly ConnectionStringInfo _connectionString;
	readonly IProviderServiceFactory<IConnectionContextFactory> _connectionContextFactory;
	readonly ITransactionManager _transactionManager;

	public DapperSql(
		ConnectionStringInfo connectionString,
		IProviderServiceFactory<IConnectionContextFactory> connectionContextFactory,
		ITransactionManager transactionManager)
	{
		_connectionString = Ensure.IsNotNull(connectionString, nameof(connectionString));
		_connectionContextFactory = Ensure.IsNotNull(connectionContextFactory, nameof(connectionContextFactory));
		_transactionManager = Ensure.IsNotNull(transactionManager, nameof(transactionManager));
	}

	/// <inheritdoc />
	public ISqlScope BeginScope()
		=> new DapperSqlScope(GetConnectionContext());

	/// <inheritdoc />
	public ISqlTransactionScope BeginTransactionScope(IsolationLevel? isolationLevel = null, TimeSpan? timeout = null)
		=> new TransactionalDapperSqlScope(GetConnectionContext(), _transactionManager, isolationLevel, timeout);

	/// <inheritdoc />
	public virtual async Task ExecuteAsync(string command, CommandSettings settings = default)
	{
		using var scope = BeginScope();
		await scope.ExecuteAsync(command, settings);
	}

	/// <inheritdoc />
	public IConnectionContext GetConnectionContext()
	{
		var factory = _connectionContextFactory.GetProviderService(
			_connectionString.ProviderId);

		return factory.CreateConnectionContext(_connectionString);
	}

	/// <inheritdoc />
	public virtual async Task<T[]> ReadAllAsync<T>(string command, CommandSettings settings = default)
	{
		using var scope = BeginScope();
		return await scope.ReadAllAsync<T>(command, settings);
	}

	/// <inheritdoc />
	public virtual async Task<T[]> ReadAllMappedAsync<T>(string command, Func<ISqlResults, Task<T[]>> mapAsync, CommandSettings settings = default)
	{
		using var scope = BeginScope();
		return await scope.ReadAllAsync<T>(command, settings);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadFirstAsync<T>(string command, CommandSettings settings = default)
	{
		using var scope = BeginScope();
		return await scope.ReadFirstAsync<T>(command, settings);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadFirstOrDefaultAsync<T>(string command, CommandSettings settings = default)
	{
		using var scope = BeginScope();
		return await scope.ReadFirstOrDefaultAsync<T>(command, settings);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadScalarAsync<T>(string command, CommandSettings settings = default)
	{
		using var scope = BeginScope();
		return await scope.ReadScalarAsync<T>(command, settings);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadSingleAsync<T>(string command, CommandSettings settings = default)
	{
		using var scope = BeginScope();
		return await scope.ReadSingleAsync<T>(command, settings);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadSingleMappedAsync<T>(string command, Func<ISqlResults, Task<T>> mapAsync, CommandSettings settings = default)
	{
		using var scope = BeginScope();
		return await scope.ReadSingleMappedAsync<T>(command, mapAsync, settings);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadSingleOrDefaultAsync<T>(string command, CommandSettings settings = default)
	{
		using var scope = BeginScope();
		return await scope.ReadSingleOrDefaultAsync<T>(command, settings);
	}

	/// <inheritdoc />
	public virtual async IAsyncEnumerable<T> StreamAllAsync<T>(string command, CommandSettings settings = default)
	{
		using var scope = BeginScope();
		await foreach (var result in scope.StreamAllAsync<T>(command, settings))
		{
			yield return result;
		}
	}
}

/// <summary>
/// Provides instances of <see cref="ISql"/> backed by Dapper.
/// </summary>
public class DapperSqlFactory : ISqlFactory
{
	readonly IConnectionStringProvider _connectionStringProvider;
	readonly IProviderServiceFactory<IConnectionContextFactory> _connectionContextFactory;
	readonly ITransactionManager _transactionManager;

	public DapperSqlFactory(
		IConnectionStringProvider connectionStringProvider,
		IProviderServiceFactory<IConnectionContextFactory> connectionContextFactory,
		ITransactionManager transactionManager)
	{
		_connectionStringProvider = Ensure.IsNotNull(connectionStringProvider, nameof(connectionStringProvider));
		_connectionContextFactory = Ensure.IsNotNull(connectionContextFactory, nameof(connectionContextFactory));
		_transactionManager = Ensure.IsNotNull(transactionManager, nameof(transactionManager));
	}

	/// <inheritdoc />
	public ISql CreateSqlContext(string connectionStringName = "default")
	{
		Ensure.IsNotNullOrEmpty(connectionStringName, nameof(connectionStringName));

		var connectionString = _connectionStringProvider.GetConnectionString(connectionStringName);

		return CreateSqlContext(connectionString);
	}

	/// <inheritdoc />
	public ISql CreateSqlContext(ConnectionStringInfo connectionString)
		=> new DapperSql(
				Ensure.IsNotNull(connectionString, nameof(connectionString)),
				_connectionContextFactory,
				_transactionManager);
}

class DapperSqlScope : Disposable, ISqlScope
{
	readonly IConnectionContext _connectionContext;

	public DapperSqlScope(IConnectionContext connectionContext)
	{
		_connectionContext = Ensure.IsNotNull(connectionContext, nameof(connectionContext));
	}

	/// <inheritdoc />
	public virtual async Task ExecuteAsync(string command, CommandSettings settings = default)
	{
		var def = CreateCommandDefinition(command, settings);

		await _connectionContext.Connection.ExecuteAsync(def);
	}

	/// <inheritdoc />
	public IConnectionContext GetConnectionContext()
		=> _connectionContext;

	/// <inheritdoc />
	public virtual async Task<T[]> ReadAllAsync<T>(string command, CommandSettings settings = default)
	{
		var def = CreateCommandDefinition(command, settings);

		return (
			await _connectionContext.Connection.QueryAsync<T>(def)
		).ToArray();
	}

	/// <inheritdoc />
	public virtual async Task<T[]> ReadAllMappedAsync<T>(string command, Func<ISqlResults, Task<T[]>> mapAsync, CommandSettings settings = default)
	{
		Ensure.IsNotNull(mapAsync, nameof(mapAsync));

		var def = CreateCommandDefinition(command, settings);

		var results = new DapperSqlResults(
			await _connectionContext.Connection.QueryMultipleAsync(def));

		return await mapAsync(results);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadFirstAsync<T>(string command, CommandSettings settings = default)
	{
		var def = CreateCommandDefinition(command, settings);

		return await _connectionContext.Connection.QueryFirstAsync<T>(def);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadFirstOrDefaultAsync<T>(string command, CommandSettings settings = default)
	{
		var def = CreateCommandDefinition(command, settings);

		return await _connectionContext.Connection.QueryFirstOrDefaultAsync<T>(def);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadScalarAsync<T>(string command, CommandSettings settings = default)
	{
		var def = CreateCommandDefinition(command, settings);

		return await _connectionContext.Connection.ExecuteScalarAsync<T>(def);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadSingleAsync<T>(string command, CommandSettings settings = default)
	{
		var def = CreateCommandDefinition(command, settings);

		return await _connectionContext.Connection.QuerySingleAsync<T>(def);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadSingleMappedAsync<T>(string command, Func<ISqlResults, Task<T>> mapAsync, CommandSettings settings = default)
	{
		Ensure.IsNotNull(mapAsync, nameof(mapAsync));

		var def = CreateCommandDefinition(command, settings);

		var results = new DapperSqlResults(
			await _connectionContext.Connection.QueryMultipleAsync(def));

		return await mapAsync(results);
	}

	/// <inheritdoc />
	public virtual async Task<T> ReadSingleOrDefaultAsync<T>(string command, CommandSettings settings = default)
	{
		var def = CreateCommandDefinition(command, settings);

		return await _connectionContext.Connection.QuerySingleOrDefaultAsync<T>(def);
	}

	/// <inheritdoc />
	public virtual async IAsyncEnumerable<T> StreamAllAsync<T>(string command, CommandSettings settings = default)
	{
		var def = CreateCommandDefinition(command, settings);

		using var reader = (DbDataReader)(await _connectionContext.Connection.ExecuteReaderAsync(def));

		var parser = reader.GetRowParser<T>();
		while (await reader.ReadAsync())
		{
			yield return parser(reader);
		}
	}

	/// <inheritdoc />
	protected override void DisposeExplicit()
	{
		_connectionContext.Dispose();
	}

	protected virtual CommandDefinition CreateCommandDefinition(string command, CommandSettings settings)
	{
		Ensure.IsNotNullOrEmpty(command, nameof(command));

		var flags = CommandFlags.Buffered;
		if (!settings.Buffered)
		{
			flags = CommandFlags.None;
		}

		return new CommandDefinition(
			commandText: command,
			commandType: settings.CommandType,
			cancellationToken: settings.CancellationToken,
			parameters: settings.Parameters,
			commandTimeout: settings.Timeout,
			flags: flags);
	}
}

class TransactionalDapperSqlScope : DapperSqlScope, ISqlTransactionScope
{
	readonly ITransactionLifetime _lifetime;

	public TransactionalDapperSqlScope(
		IConnectionContext connectionContext,
		ITransactionManager transactionManager,
		IsolationLevel? isolationLevel,
		TimeSpan? timeout)
		: base(connectionContext)
	{
		_lifetime = Ensure.IsNotNull(transactionManager, nameof(transactionManager))
			.BeginTransaction(isolationLevel, timeout);
	}

	/// <inheritdoc />
	public Task CommitAsync(CancellationToken cancellationToken = default)
		=> _lifetime.CommitAsync(cancellationToken);

	/// <inheritdoc />
	public Task RollbackAsync(CancellationToken cancellationToken = default)
		=> _lifetime.RollbackAsync(cancellationToken);

	/// <inheritdoc />
	public override Task ExecuteAsync(string command, CommandSettings settings = default)
		=> RunCoreAsync(() => base.ExecuteAsync(command, settings));

	/// <inheritdoc />
	public override Task<T[]> ReadAllAsync<T>(string command, CommandSettings settings = default)
		=> RunCoreAsync(() => base.ReadAllAsync<T>(command, settings));

	/// <inheritdoc />
	public override Task<T[]> ReadAllMappedAsync<T>(string command, Func<ISqlResults, Task<T[]>> mapAsync, CommandSettings settings = default)
		=> RunCoreAsync(() => base.ReadAllMappedAsync<T>(command, mapAsync, settings));

	/// <inheritdoc />
	public override Task<T> ReadFirstAsync<T>(string command, CommandSettings settings = default)
		=> RunCoreAsync(() => base.ReadFirstAsync<T>(command, settings));

	/// <inheritdoc />
	public override Task<T> ReadFirstOrDefaultAsync<T>(string command, CommandSettings settings = default)
		=> RunCoreAsync(() => base.ReadFirstOrDefaultAsync<T>(command, settings));

	/// <inheritdoc />
	public override Task<T> ReadScalarAsync<T>(string command, CommandSettings settings = default)
		=> RunCoreAsync(() => base.ReadScalarAsync<T>(command, settings));

	/// <inheritdoc />
	public override Task<T> ReadSingleAsync<T>(string command, CommandSettings settings = default)
		=> RunCoreAsync(() => base.ReadSingleAsync<T>(command, settings));

	/// <inheritdoc />
	public override Task<T> ReadSingleMappedAsync<T>(string command, Func<ISqlResults, Task<T>> mapAsync, CommandSettings settings = default)
		=> RunCoreAsync(() => base.ReadSingleMappedAsync<T>(command, mapAsync, settings));

	/// <inheritdoc />
	public override Task<T> ReadSingleOrDefaultAsync<T>(string command, CommandSettings settings = default)
		=> RunCoreAsync(() => base.ReadSingleOrDefaultAsync<T>(command, settings));

	async Task RunCoreAsync(Func<Task> task)
	{
		try
		{
			await task();
		}
		catch
		{
			await RollbackAsync();

			throw;
		}
	}

	async Task<T> RunCoreAsync<T>(Func<Task<T>> task)
	{
		try
		{
			return await task();
		}
		catch
		{
			await RollbackAsync();

			throw;
		}
	}

	protected override void DisposeExplicit()
	{
		base.DisposeExplicit();

		_lifetime.Dispose();
	}
}

class DapperSqlResults : Disposable, ISqlResults
{
	readonly SqlMapper.GridReader _reader;

	public DapperSqlResults(SqlMapper.GridReader reader)
	{
		_reader = Ensure.IsNotNull(reader, nameof(reader));
	}

	/// <inheritdoc />
	public async Task<T[]> ReadAllAsync<T>()
		=> (await _reader.ReadAsync<T>()).ToArray();

	/// <inheritdoc />
	public Task<T> ReadFirstAsync<T>()
		=> _reader.ReadFirstAsync<T>();

	/// <inheritdoc />
	public Task<T?> ReadFirstOrDefaultAsync<T>()
		=> _reader.ReadFirstOrDefaultAsync<T>()!;

	/// <inheritdoc />
	public Task<T> ReadSingleAsync<T>()
		=> _reader.ReadSingleAsync<T>();

	/// <inheritdoc />
	public Task<T?> ReadSingleOrDefaultAsync<T>()
		=> _reader.ReadSingleOrDefaultAsync<T>()!;

	/// <inheritdoc />
	protected override void DisposeExplicit()
	{
		_reader.Dispose();
	}
}