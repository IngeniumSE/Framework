using Ingenium.Platform.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ingenium.Platform.Membership;

/// <summary>
/// Represents a user.
/// </summary>
public abstract class Role<TRoleId> : Entity
	where TRoleId : struct
{
	/// <summary>
	/// Gets or sets the user ID.
	/// </summary>
	public TRoleId RoleId { get; set; }

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	public string Name { get; set; } = default!;
}

public abstract class RoleEntityTypeConfiguration<TRole, TRoleId, TRoleIdValueConverter> : EntityTypeConfiguration<TRole>
	where TRole : Role<TRoleId>
	where TRoleId : struct
{
	readonly string _table;
	readonly string _schema;

	protected RoleEntityTypeConfiguration(string table, string schema)
	{
		_table = Ensure.IsNotNullOrEmpty(table, nameof(table));
		_schema = Ensure.IsNotNullOrEmpty(schema, nameof(schema));
	}

	/// <inheritdoc />
	protected override void ConfigureDomain(EntityTypeBuilder<TRole> builder)
	{
		builder.ToTable(_table, _schema);

		builder.HasKey(e => e.RoleId);

		builder.Property(e => e.RoleId)
			.IsRequired()
			.ValueGeneratedOnAdd()
			.HasConversion<TRoleIdValueConverter>();

		builder.Property(e => e.Name)
			.IsRequired()
			.HasMaxLength(512)
			.IsUnicode();
	}
}

public interface IRoleReader<TRole, TRoleId> : IReader<TRole, TRoleId>
	where TRole : Role<TRoleId>
	where TRoleId : struct
{ }

public abstract class RoleReader<TRole, TRoleId, TContext> : Reader<TContext, TRole, TRoleId>, IRoleReader<TRole, TRoleId>
	where TRole : Role<TRoleId>
	where TRoleId : struct
	where TContext : DbContext
{
	public RoleReader(TContext context) : base(context) { }
}