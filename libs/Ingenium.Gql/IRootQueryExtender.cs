using GraphQL.Types;

namespace Ingenium.Gql;

/// <summary>
/// Defines the required contract for implementing a root query extender.
/// </summary>
public interface IRootQueryExtender
{
	/// <summary>
	/// Extends the given query.
	/// </summary>
	/// <param name="query">The query graph type.</param>
	void ExtendQuery(IObjectGraphType query);
}
