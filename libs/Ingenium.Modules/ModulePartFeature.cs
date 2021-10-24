using System.Reflection;

using Ingenium.Parts;

namespace Ingenium.Modules;

/// <summary>
/// Collects type information for available modules from parts.
/// </summary>
public class ModulePartFeature
{
	/// <summary>
	/// Gets the set of module types.
	/// </summary>
	public List<TypeInfo> ModuleTypes { get; } = new();
}

/// <summary>
/// Provides discovery of module types from application parts.
/// </summary>
public class ModulePartFeatureProvider : IPartFeatureProvider<ModulePartFeature>
{
	static readonly Type ModuleType = typeof(IModule);

	/// <inheritdoc />
	public void PopulateFeature(IEnumerable<IPart> parts, ModulePartFeature feature)
	{
		Ensure.IsNotNull(parts, nameof(parts));
		Ensure.IsNotNull(feature, nameof(feature));

		foreach (var part in parts)
		{
			if (part is IPartTypeProvider provider)
			{
				foreach (var type in provider.Types)
				{
					if (IsModule(type))
					{
						feature.ModuleTypes.Add(type);
					}
				}
			}
		}
	}

	static bool IsModule(TypeInfo typeInfo)
	{
		var type = typeInfo.AsType();

		return type.IsPublic
			&& !type.IsAbstract
			&& type.IsAssignableTo(ModuleType);
	}
}