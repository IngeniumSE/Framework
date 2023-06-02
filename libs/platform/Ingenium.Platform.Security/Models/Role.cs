using Ingenium.Platform.Membership;

namespace Ingenium.Platform.Security;

using static GenerateIdFeatures;

[GenerateId(
	features: EFValueConverter | SystemTextJsonConverter
)]
public partial struct RoleId { }

/// <summary>
/// Represents a user.
/// </summary>
public class Role : Role<RoleId> { }

public class RoleEntityTypeConfiguration : RoleEntityTypeConfiguration<Role, RoleId, RoleIdEFValueConverter>
{
	public RoleEntityTypeConfiguration()
		: base("Role", "sec") { }
}

public interface IRoleReader : IRoleReader<Role, RoleId> { }

public class RoleReader : RoleReader<Role, RoleId, SecurityDbContext>, IRoleReader
{
	public RoleReader(SecurityDbContext context) : base(context) { }
}