using GraphQL;
using GraphQL.Types;

using Microsoft.Extensions.DependencyInjection;

namespace Ingenium.Gql;

/// <summary>
/// Provides extensions for the <see cref="ObjectGraphType"/> type.
/// </summary>
public static class ObjectGraphTypeExtensions
{
	/// <summary>
	/// Defines a GraphQL collection query.
	/// </summary>
	/// <typeparam name="TGraphType">The graph type.</typeparam>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <param name="graph">The graph.</param>
	/// <param name="name">The collection name.</param>
	/// <param name="resolve">The resolver.</param>
	/// <returns>The field type.</returns>
	public static FieldType Collection<TGraphType, TService>(
		this ObjectGraphType graph,
		string name,
		Func<IResolveFieldContext<object>, TService, object> resolve)
		where TGraphType : IGraphType
		where TService : notnull
	{
		Ensure.IsNotNull(graph, nameof(graph));
		Ensure.IsNotNullOrEmpty(name, nameof(name));
		Ensure.IsNotNull(resolve, nameof(resolve));

		return graph.Field<ListGraphType<TGraphType>>(
			name: name,
			resolve: context =>
			{
				var service = context.RequestServices!.GetRequiredService<TService>();

				return resolve(context!, service);
			});
	}

	/// <summary>
	/// Defines a GraphQL collection query.
	/// </summary>
	/// <typeparam name="TGraphType">The graph type.</typeparam>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <param name="graph">The graph.</param>
	/// <param name="name">The collection name.</param>
	/// <param name="args">The query arguments builder.</param>
	/// <param name="resolve">The resolver.</param>
	/// <returns>The field type.</returns>
	public static FieldType Collection<TGraphType, TService>(
		this ObjectGraphType graph,
		string name,
		Action<QueryArgumentsBuilder> args,
		Func<IResolveFieldContext<object>, TService, object> resolve)
		where TGraphType : IGraphType
		where TService : notnull
	{
		Ensure.IsNotNull(graph, nameof(graph));
		Ensure.IsNotNullOrEmpty(name, nameof(name));
		Ensure.IsNotNull(args, nameof(args));
		Ensure.IsNotNull(resolve, nameof(resolve));

		var builder = new QueryArgumentsBuilder();
		args(builder);

		return graph.Field<ListGraphType<TGraphType>>(
			name: name,
			arguments: builder.Build(),
			resolve: context =>
			{
				var service = context.RequestServices!.GetRequiredService<TService>();

				return resolve(context!, service);
			});
	}

	/// <summary>
	/// Defines a GraphQL item query.
	/// </summary>
	/// <typeparam name="TGraphType">The graph type.</typeparam>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <param name="graph">The graph.</param>
	/// <param name="name">The collection name.</param>
	/// <param name="resolve">The resolver.</param>
	/// <returns>The field type.</returns>
	public static FieldType Item<TGraphType, TService>(
		this ObjectGraphType graph,
		string name,
		Func<IResolveFieldContext<object>, TService, object> resolve)
		where TGraphType : IGraphType
		where TService : notnull
	{
		Ensure.IsNotNull(graph, nameof(graph));
		Ensure.IsNotNullOrEmpty(name, nameof(name));
		Ensure.IsNotNull(resolve, nameof(resolve));

		return graph.Field<TGraphType>(
			name: name,
			resolve: context =>
			{
				var service = context.RequestServices!.GetRequiredService<TService>();

				return resolve(context!, service);
			});
	}

	/// <summary>
	/// Defines a GraphQL item query.
	/// </summary>
	/// <typeparam name="TGraphType">The graph type.</typeparam>
	/// <typeparam name="TService">The service type.</typeparam>
	/// <param name="graph">The graph.</param>
	/// <param name="name">The collection name.</param>
	/// <param name="args">The query arguments builder.</param>
	/// <param name="resolve">The resolver.</param>
	/// <returns>The field type.</returns>
	public static FieldType Item<TGraphType, TService>(
		this ObjectGraphType graph,
		string name,
		Action<QueryArgumentsBuilder> args,
		Func<IResolveFieldContext<object>, TService, object> resolve)
		where TGraphType : IGraphType
		where TService : notnull
	{
		Ensure.IsNotNull(graph, nameof(graph));
		Ensure.IsNotNullOrEmpty(name, nameof(name));
		Ensure.IsNotNull(args, nameof(args));
		Ensure.IsNotNull(resolve, nameof(resolve));

		var builder = new QueryArgumentsBuilder();
		args(builder);

		return graph.Field<TGraphType>(
			name: name,
			arguments: builder.Build(),
			resolve: context =>
			{
				var service = context.RequestServices!.GetRequiredService<TService>();

				return resolve(context!, service);
			});
	}
}
