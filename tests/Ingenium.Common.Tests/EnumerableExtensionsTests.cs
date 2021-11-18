using Xunit;

namespace Ingenium.Common;

public class EnumerableExtensionsTests
{
	[Fact]
	public void OrderByDependencies_Should_ValidateArguments()
	{
		// Arrange
		var source = Enumerable.Empty<string>();
		string keySelector(string element) => element;
		IEnumerable<string> dependentSelector(string element, string dependencyKey) => Enumerable.Empty<string>();

		// Act, Assert
		Assert.Throws<ArgumentNullException>(
			"source",
			() => EnumerableExtensions.OrderByDependencies<string, string>(
				null! /* source */,
				keySelector,
				dependentSelector));

		Assert.Throws<ArgumentNullException>(
			"keySelector",
			() => EnumerableExtensions.OrderByDependencies<string, string>(
				source,
				null! /* keySelector */,
				dependentSelector));

		Assert.Throws<ArgumentNullException>(
			"dependentSelector",
			() => EnumerableExtensions.OrderByDependencies<string, string>(
				source,
				keySelector,
				null! /* dependentSelector */));
	}

	[Fact]
	public void OrderByDependencies_Should_SortElements_ByDependencies()
	{
		// Arrange
		Item[] source =
		{
			new("One"),
			new("Two", "Three", "Four"),
			new("Three", "One"),
			new("Four")
		};

		// Act
		var sorted = source
			.OrderByDependencies(i => i.Key, (i, k) => i.Dependencies)
			.ToList();

		// Assert
		Assert.Equal(4, sorted.Count);
		Assert.Same(source[0], sorted[0]);
		Assert.Same(source[2], sorted[1]);
		Assert.Same(source[3], sorted[2]);
		Assert.Same(source[1], sorted[3]);
	}

	[Fact]
	public void OrderByDependencies_Should_ThrowException_ForCyclicDependency()
	{
		// Arrange
		Item[] source =
		{
			new("One"),
			new("Two", "Three", "Four"),
			new("Three", "One"),
			new("Four", "Two")
		};

		// Act
		var ioe = Assert.Throws<InvalidOperationException>(
			() =>
			{
				var sorted = source
					.OrderByDependencies(i => i.Key, (i, k) => i.Dependencies)
					.ToList();
			});

		// Assert
		var expectedMessage = string.Format(
			CommonResources.CyclicDependencyExceptionMessage,
			"Two => Four => Two",
			"Two");
		Assert.Equal(expectedMessage, ioe.Message);
	}

	[Fact]
	public void OrderByDependencies_Should_ThrowException_ForMissingDependency()
	{
		// Arrange
		Item[] source =
		{
			new("One"),
			new("Two", "Three", "Four"),
			new("Three", "One"),
			new("Four", "Five")
		};

		// Act
		var ioe = Assert.Throws<InvalidOperationException>(
			() =>
			{
				var sorted = source
					.OrderByDependencies(i => i.Key, (i, k) => i.Dependencies)
					.ToList();
			});

		// Assert
		var expectedMessage = string.Format(
			CommonResources.MissingDependencyExceptionMessage,
			"Two => Four => Five",
			"Five");
		Assert.Equal(expectedMessage, ioe.Message);
	}

	record Item(string Key, params string[] Dependencies);
}
