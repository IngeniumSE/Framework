using Ingenium.Platform.Data;
using Ingenium.Platform.Membership;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ingenium.Platform.Security.Users;

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