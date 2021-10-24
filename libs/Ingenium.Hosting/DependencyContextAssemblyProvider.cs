using System.Collections.ObjectModel;
using System.Reflection;

using Microsoft.Extensions.DependencyModel;

namespace Ingenium.Hosting;

/// <summary>
/// Provides assemblies resolved from a dependency context.
/// </summary>
public class DependencyContextAssemblyProvider
{
	readonly DependencyContext _context;
	readonly Lazy<ReadOnlyCollection<Assembly>> _assemblies;

	internal static HashSet<string> ReferencedAssemblies = new(StringComparer.OrdinalIgnoreCase)
	{
		"Ingenium.Modules",
		"Ingenium.Parts"
	};

	/// <summary>
	/// Initialises a new instance of <see cref="DependencyContextAssemblyProvider"/>
	/// </summary>
	/// <param name="context">The dependency context.</param>
	public DependencyContextAssemblyProvider(DependencyContext context)
	{
		_context = Ensure.IsNotNull(context, nameof(context));
		_assemblies = new Lazy<ReadOnlyCollection<Assembly>>(() => FindAssemblies());
	}

	/// <summary>
	/// Gets the set of assemblies.
	/// </summary>
	public IReadOnlyCollection<Assembly> Assemblies => _assemblies.Value;

	class CandidateResolver
	{
		readonly IDictionary<string, Dependency> _runtimeDependencies;

		public CandidateResolver(
			IReadOnlyList<RuntimeLibrary> runtimeDependencies,
			ISet<string> referenceAssemblies)
		{
			var dependenciesWithNoDuplicates = new Dictionary<string, Dependency>(StringComparer.OrdinalIgnoreCase);
			foreach (var dependency in runtimeDependencies)
			{
				if (!dependenciesWithNoDuplicates.TryGetValue(dependency.Name, out _))
				{
					dependenciesWithNoDuplicates.Add(dependency.Name, CreateDependency(dependency, referenceAssemblies));
				}
			}

			_runtimeDependencies = dependenciesWithNoDuplicates;
		}

		Dependency CreateDependency(RuntimeLibrary library, ISet<string> referenceAssemblies)
		{
			var classification = Classification.Unknown;
			if (referenceAssemblies.Contains(library.Name))
			{
				classification = Classification.FrameworkReference;
			}

			return new Dependency(library, classification);
		}
	}
	record Dependency(RuntimeLibrary Library, Classification Classification);
	enum Classification
	{
		Unknown = 0,
		ReferencesFramework = 1,
		DoesNotReferenceFramework = 2,
		FrameworkReference = 3
	}
}