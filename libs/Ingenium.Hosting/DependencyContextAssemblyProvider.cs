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

	ReadOnlyCollection<Assembly> FindAssemblies()
	{
		var assemblies = GetCandidateLibraries()
			.SelectMany(rl => rl.GetDefaultAssemblyNames(_context))
			.Select(rl => Assembly.Load(rl))
			.ToList();

		return new ReadOnlyCollection<Assembly>(assemblies);
	}

	IEnumerable<RuntimeLibrary> GetCandidateLibraries()
	{
		if (ReferencedAssemblies is null)
		{
			return Enumerable.Empty<RuntimeLibrary>();
		}

		var resolver = new CandidateResolver(_context.RuntimeLibraries, ReferencedAssemblies);

		return resolver.GetCandidates();
	}

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

		public IEnumerable<RuntimeLibrary> GetCandidates()
		{
			foreach (var (name, dependency) in _runtimeDependencies)
			{
				var classification = ComputeClassification(name);
				if (classification == Classification.ReferencesFramework
					|| classification == Classification.FrameworkReference)
				{
					yield return dependency.Library;
				}
			}
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

		Classification ComputeClassification(string dependency)
		{
			if (!_runtimeDependencies.TryGetValue(dependency, out var candidate))
			{
				return Classification.DoesNotReferenceFramework;
			}

			if (candidate.Classification != Classification.Unknown)
			{
				return candidate.Classification;
			}

			var classification = Classification.DoesNotReferenceFramework;
			foreach (var candidateDependency in candidate.Library.Dependencies)
			{
				var dependencyClassification = ComputeClassification(candidateDependency.Name);
				if (dependencyClassification == Classification.ReferencesFramework
					|| dependencyClassification == Classification.FrameworkReference)
				{
					classification = Classification.ReferencesFramework;
					break;
				}
			}

			candidate.Classification = classification;

			return classification;
		}

	}
	class Dependency
	{
		public Dependency(
			RuntimeLibrary library, 
			Classification classification)
		{
			Library = library;
			Classification = classification;
		}

		public RuntimeLibrary Library { get; }
		public Classification Classification { get; internal set; }
	}
	enum Classification
	{
		Unknown = 0,
		ReferencesFramework = 1,
		DoesNotReferenceFramework = 2,
		FrameworkReference = 3
	}
}