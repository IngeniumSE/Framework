using GraphQL.Types;

namespace Ingenium.Gql;

/// <summary>
/// Provides build services for creating query arguments.
/// </summary>
public class QueryArgumentsBuilder
{
	readonly List<QueryArgument> _arguments = new();

	/// <summary>
	/// Builds the query arguments.
	/// </summary>
	/// <returns>The query arguments.</returns>
	public QueryArguments Build()
		=> new(_arguments);

	/// <summary>
	/// Adds a <see cref="bool"/> argument to the query.
	/// </summary>
	/// <param name="name">The query argument name.</param>
	/// <param name="description">The query argument description.</param>
	/// <param name="nullable">Is the argument nullable?</param>
	/// <returns>The query arguments build.</returns>
	public QueryArgumentsBuilder Boolean(
		string name,
		string? description = default,
		bool nullable = false)
		=> AddArgumentType<BooleanGraphType>(name, description, nullable);

	/// <summary>
	/// Adds a <see cref="bool"/> argument to the query.
	/// </summary>
	/// <param name="name">The query argument name.</param>
	/// <param name="description">The query argument description.</param>
	/// <param name="nullable">Is the argument nullable?</param>
	/// <returns>The query arguments build.</returns>
	public QueryArgumentsBuilder Enum<TEnum>(
		string name,
		string? description = default,
		bool nullable = false)
		where TEnum : Enum
		=> AddArgumentType<EnumerationGraphType<TEnum>>(name, description, nullable);

	/// <summary>
	/// Adds a <see cref="int"/> argument to the query.
	/// </summary>
	/// <param name="name">The query argument name.</param>
	/// <param name="description">The query argument description.</param>
	/// <param name="nullable">Is the argument nullable?</param>
	/// <returns>The query arguments build.</returns>
	public QueryArgumentsBuilder Int32(
		string name, 
		string? description = default,
		bool nullable = false)
		=> AddArgumentType<IntGraphType>(name, description, nullable);

	/// <summary>
	/// Adds a <see cref="string"/> argument to the query.
	/// </summary>
	/// <param name="name">The query argument name.</param>
	/// <param name="description">The query argument description.</param>
	/// <param name="nullable">Is the argument nullable?</param>
	/// <returns>The query arguments build.</returns>
	public QueryArgumentsBuilder String(
		string name,
		string? description = default,
		bool nullable = false)
		=> AddArgumentType<StringGraphType>(name, description, nullable);

	QueryArgumentsBuilder AddArgumentType<TGraphType>(
		string name,
		string? description = default,
		bool nullable = false)
		where TGraphType : IGraphType
	{
		QueryArgument queryArgument = nullable
			? new QueryArgument<TGraphType>()
			: new QueryArgument<NonNullGraphType<TGraphType>>();

		queryArgument.Name = name;
		queryArgument.Description = description;

		_arguments.Add(queryArgument);

		return this;
	}
}
