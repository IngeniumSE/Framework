using Microsoft.EntityFrameworkCore;

namespace Ingenium.Platform.Security;

/// <summary>
/// Provides a database context for security.
public partial class SecurityDbContext : DbContext
{
	public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
		: base(options) { }

	public DbSet<User> Users { get; set; } = default!;

	public DbSet<Role> Roles { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		Ensure.IsNotNull(modelBuilder, nameof(modelBuilder));

		modelBuilder.ApplyConfiguration<User, UserEntityTypeConfiguration>();
		modelBuilder.ApplyConfiguration<Role, RoleEntityTypeConfiguration>();
	}
}
