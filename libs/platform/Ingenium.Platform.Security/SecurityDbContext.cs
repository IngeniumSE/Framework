using Ingenium.Platform.Security.Users;

using Microsoft.EntityFrameworkCore;

namespace Ingenium.Platform.Security;

/// <summary>
/// Provides a database context for security.
public partial class SecurityDbContext : DbContext
{
	public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
		: base(options) { }

	public DbSet<User> Users { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<User>());
	}
}
