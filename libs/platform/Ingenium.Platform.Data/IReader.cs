using Microsoft.EntityFrameworkCore;

namespace Ingenium.Platform.Data;

/// <summary>
/// Defines the required contract for implementing a reader.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The key type.</typeparam>
public interface IReader<TEntity, in TKey>
	where TEntity : Entity
{
	/// <summary>
	/// Gets the entity with the provided key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	ValueTask<TEntity?> FindByKeyAsync(TKey key, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines the required contract for implementing a reader.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IReader<TEntity> : IReader<TEntity, int>
	where TEntity : Entity
{ }

/// <summary>
/// Provides a default implementation of a reader.
/// </summary>
/// <typeparam name="TContext">The context type.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The key type.</typeparam>
public class Reader<TContext, TEntity, TKey> : IReader<TEntity, TKey>
	where TContext : DbContext
	where TEntity : Entity
{
	protected readonly TContext _context;

	public Reader(TContext context)
	{
		_context = Ensure.IsNotNull(context, nameof(context));
	}

	/// <inheritdoc />
	public ValueTask<TEntity?> FindByKeyAsync(TKey key, CancellationToken cancellationToken = default)
		=> _context.Set<TEntity>().FindAsync(key);
}

/// <summary>
/// Provides a default implementation of a reader.
/// </summary>
/// <typeparam name="TContext">The context type.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class Reader<TContext, TEntity> : Reader<TContext, TEntity, int>, IReader<TEntity>
	where TContext : DbContext
	where TEntity : Entity
{
	public Reader(TContext context) : base(context) { }
}