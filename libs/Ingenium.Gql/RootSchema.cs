using GraphQL.Types;

namespace Ingenium.Gql;

/// <summary>
/// Represetnts the root schema for all GraphQL operations.
/// </summary>
public class RootSchema : Schema
{
	public RootSchema(
		GqlOptions options,
		IRootQueryFactory rootQueryFactory,
		IEnumerable<IRootSchemaExtender> schemaExtenders)
	{
		Ensure.IsNotNull(options, nameof(options));
		Ensure.IsNotNull(rootQueryFactory, nameof(rootQueryFactory));
		Ensure.IsNotNull(schemaExtenders, nameof(schemaExtenders));

		Query = rootQueryFactory.CreateQuery(options.RootQueryName);

		foreach (var extender in schemaExtenders)
		{
			extender.ExtendSchema(this);
		}
	}
}
