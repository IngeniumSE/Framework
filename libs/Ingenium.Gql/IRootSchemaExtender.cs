using GraphQL.Types;

namespace Ingenium.Gql;

/// <summary>
/// Defines the required contract for implementing a root schema extender.
/// </summary>
public interface IRootSchemaExtender
{
	/// <summary>
	/// Extends the given schema.
	/// </summary>
	/// <param name="schema">The schema instance.</param>
	void ExtendSchema(Schema schema);
}
