using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ingenium.Tenants;

/// <summary>
/// Represents a tenant ID.
/// </summary>
[GenerateId(typeof(string), caseInsensitive: true)]
public partial struct TenantId
{
	/// <summary>
	/// Represents the default tenant.
	/// </summary>
	public static readonly TenantId Default = new TenantId("Default");

	/// <summary>
	/// Gets or sets whether the tenant ID is an empty tenant ID.
	/// </summary>
	/// <returns></returns>
	public bool IsEmpty() => !HasValue;

	/// <summary>
	/// Gets or sets whether the tenant ID is the default tenant ID.
	/// </summary>
	/// <returns></returns>
	public bool IsDefault() => string.Equals("Default", Value, StringComparison.OrdinalIgnoreCase);

	/// <summary>
	/// Gets or sets whether the tenant ID is the default, or an empty tenant ID.
	/// </summary>
	/// <returns></returns>
	public bool IsDefaultOrEmpty() => IsEmpty() || IsDefault();
}

/// <summary>
/// Represents a tenant-scoped ID.
/// </summary>
/// <typeparam name="TId">The scoped ID type.</typeparam>
/// <param name="TenantId">The tenant ID.</param>
/// <param name="ScopeId">The scoped ID.</param>
public record struct TenantScopedId<TId>(TenantId TenantId, TId ScopeId) where TId : struct
{
	public static IEqualityComparer<TenantScopedId<TId>> Comparer = new _Comparer();

	class _Comparer : IEqualityComparer<TenantScopedId<TId>>
	{
		public bool Equals(TenantScopedId<TId> x, TenantScopedId<TId> y)
			=> x.TenantId.Equals(y.TenantId) && x.ScopeId.Equals(y.ScopeId);

		public int GetHashCode([DisallowNull] TenantScopedId<TId> obj)
			=> HashCode.Combine(obj.TenantId, obj.ScopeId);
	}
}