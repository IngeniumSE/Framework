using System.Collections;

namespace Ingenium;

/// <summary>
/// Provides extensions for the <see cref="IEnumerable{T}"/> type.
/// </summary>
public static class EnumerableExtensions
{
	/// <summary>
	/// Orders a given set of items by dependencies.
	/// </summary>
	/// <typeparam name="TElement">The element type.</typeparam>
	/// <typeparam name="TDependencyKey">The dependency key type.</typeparam>
	/// <param name="source">The source set.</param>
	/// <param name="keySelector">The key selector.</param>
	/// <param name="dependentSelector">THe dependent key selector.</param>
	/// <returns>The ordered set of items.</returns>
	public static IEnumerable<TElement> OrderByDependencies<TElement, TDependencyKey>(
		this IEnumerable<TElement> source,
		Func<TElement, TDependencyKey> keySelector,
		Func<TElement, TDependencyKey, IEnumerable<TDependencyKey>?> dependentSelector)
		where TDependencyKey : notnull
		=> new DependencyKeyOrderedEnumerable<TElement, TDependencyKey>(source, keySelector, dependentSelector);

	class DependencyKeyOrderedEnumerable<TElement, TDependencyKey> : IEnumerable<TElement>
		where TDependencyKey : notnull
	{
		const string DependencySeperator = " => ";

		readonly IEnumerable<TElement> _source;
		readonly Func<TElement, TDependencyKey> _keySelector;
		readonly Func<TElement, TDependencyKey,  IEnumerable<TDependencyKey>?> _dependentSelector;

		public DependencyKeyOrderedEnumerable(
			IEnumerable<TElement> source,
			Func<TElement, TDependencyKey> keySelector,
			Func<TElement, TDependencyKey, IEnumerable<TDependencyKey>?> dependentSelector)
		{
			_source = Ensure.IsNotNull(source, nameof(source));
			_keySelector = Ensure.IsNotNull(keySelector, nameof(keySelector));
			_dependentSelector = Ensure.IsNotNull(dependentSelector, nameof(dependentSelector));
		}

		/// <inheritdoc />
		public IEnumerator<TElement> GetEnumerator()
			=> new DependencyKeyOrderedEnumerator(_source, _keySelector, _dependentSelector);
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		class DependencyKeyOrderedEnumerator : IEnumerator<TElement>
		{
			readonly Dictionary<TDependencyKey, TElement> _source;
			readonly Func<TElement, TDependencyKey, IEnumerable<TDependencyKey>?> _dependentSelector;
			readonly IEnumerator<TElement> _output;

			public DependencyKeyOrderedEnumerator(
				IEnumerable<TElement> source,
				Func<TElement, TDependencyKey> keySelector,
				Func<TElement, TDependencyKey, IEnumerable<TDependencyKey>?> dependentSelector)
			{
				_source = source.ToDictionary(e => keySelector(e));
				_dependentSelector = dependentSelector;

				_output = SortDependencies();
			}

			public TElement Current => _output.Current;
			object IEnumerator.Current => Current!;
			public void Dispose() => _output.Dispose();
			public bool MoveNext() => _output.MoveNext();
			public void Reset() => _output.Reset();

			IEnumerator<TElement> SortDependencies()
			{
				var sorted = new HashSet<TElement>();
				var visited = new Dictionary<TDependencyKey, bool>();
				var stack = new Stack<TDependencyKey>();

				foreach (var (key, dependency) in _source)
				{
					Visit(dependency, key, sorted, visited, stack);
				}

				return sorted.GetEnumerator();
			}

			void Visit(
				TElement element, 
				TDependencyKey key, 
				HashSet<TElement> sorted, 
				Dictionary<TDependencyKey, bool> visited,
				Stack<TDependencyKey> stack)
			{
				if (visited.TryGetValue(key, out bool visiting))
				{
					if (visiting)
					{
						throw new InvalidOperationException(
							string.Format(
								CommonResources.CyclicDependencyExceptionMessage,
								string.Join(DependencySeperator, stack.Reverse().Concat(new[] { key })),
								key));
					}
				}
				else
				{
					visited[key] = true;
					stack.Push(key);

					var dependencies = _dependentSelector(element, key);
					if (dependencies is not null)
					{
						foreach (var dependency in dependencies)
						{
							if (_source.TryGetValue(dependency, out var dependent))
							{
								Visit(dependent, dependency, sorted, visited, stack);
							}
							else
							{
								throw new InvalidOperationException(
									string.Format(
										CommonResources.MissingDependencyExceptionMessage,
										string.Join(DependencySeperator, stack.Reverse().Concat(new[] { dependency })),
										dependency));
							}
						}
					}

					stack.Pop();
					visited[key] = false;
					sorted.Add(element);
				}
			}
		}
	}
}
