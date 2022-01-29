using GraphQL.Types;

namespace Ingenium.Gql;

/// <summary>
/// Provides extensions for the <see cref="Schema"/> type.
/// </summary>
public static class SchemaExtensions
{
	/// <summary>
	/// Registers a type mapping for the given model to a GraphQL type.
	/// </summary>
	/// <typeparam name="TModel">The model type.</typeparam>
	/// <typeparam name="TGraphType">The graph type.</typeparam>
	/// <param name="schema">The schema.</param>
	/// <returns>The schema instance.</returns>
	public static Schema RegisterTypeMapping<TModel, TGraphType>(this Schema schema)
		where TModel : class
		where TGraphType : ObjectGraphType<TModel>
	{
		Ensure.IsNotNull(schema, nameof(schema));

		schema.RegisterTypeMapping(typeof(TModel), typeof(TGraphType));

		return schema;
	}

	/// <summary>
	/// Registers a type mapping for an enum.
	/// </summary>
	/// <typeparam name="TEnum">The enum type.</typeparam>
	/// <typeparam name="TGraphType">The graph type.</typeparam>
	/// <param name="schema">The schema.</param>
	/// <returns>The schema instance.</returns>
	public static Schema RegisterEnumerationTypeMapping<TEnum, TGraphType>(this Schema schema)
		where TEnum : Enum
		where TGraphType : EnumerationGraphType<TEnum>
	{
		Ensure.IsNotNull(schema, nameof(schema));

		schema.RegisterTypeMapping(typeof(TEnum), typeof(TGraphType));

		return schema;
	}
}
