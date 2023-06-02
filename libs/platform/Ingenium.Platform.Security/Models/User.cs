using Ingenium.Platform.Data;
using Ingenium.Platform.Membership;

namespace Ingenium.Platform.Security;

/// <summary>
/// Represents a user.
/// </summary>
public class User : User<UserId> { }

public class UserEntityTypeConfiguration : UserEntityTypeConfiguration<User, UserId, UserIdEFValueConverter>
{
	public UserEntityTypeConfiguration()
		: base("User", "sec") { }
}

public interface IUserReader : IUserReader<User, UserId> { }

public class UserReader : UserReader<User, UserId, SecurityDbContext>, IUserReader
{
	public UserReader(SecurityDbContext context) : base(context) { }
}