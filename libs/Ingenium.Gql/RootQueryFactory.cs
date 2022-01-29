using GraphQL.Types;

namespace Ingenium.Gql;

/// <summary>
/// Defines the required contract for implementing a query factory.
/// </summary>
public interface IRootQueryFactory
{
	/// <summary>
	/// Creates the query.
	/// </summary>
	/// <param name="name">The query name.</param>
	/// <returns>The query as a graph type.</returns>
	IObjectGraphType CreateQuery(string name = "Root");
}

/// <summary>
/// Represents a query factory.
/// </summary>
public class RootQueryFactory : IRootQueryFactory
{
	readonly IEnumerable<IRootQueryExtender> _extenders;

	public RootQueryFactory(IEnumerable<IRootQueryExtender> extenders)
	{
		_extenders = Ensure.IsNotNull(extenders, nameof(extenders));
	}

	/// <inheritdoc />
	public IObjectGraphType CreateQuery(string name = "Root")
	{
		Ensure.IsNotNullOrEmpty(name, nameof(name));

		var query = new Query(name);

		foreach (var extender in _extenders)
		{
			extender.ExtendQuery(query);
		}

		return query;
	}

	class Query : ObjectGraphType
	{
		public Query(string name)
		{
			Name = name;
		}
	}
}
