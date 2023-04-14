using Ingenium.Platform.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ingenium.Platform.Membership;

/// <summary>
/// Represents a user.
/// </summary>
public abstract class User<TUserId> : Entity
	where TUserId : struct
{
	/// <summary>
	/// Gets or sets the user ID.
	/// </summary>
	public TUserId UserId { get; set; }

	/// <summary>
	/// Gets or sets the email.
	/// </summary>
	public EmailAddress Email { get; set; } = default!;

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	public string FormalName { get; set; } = default!;

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	/// Gets or sets the user's handle.
	/// </summary>
	public string? Handle { get; set; }
}

public abstract class UserEntityTypeConfiguration<TUser, TUserId, TUserIdValueConverter> : EntityTypeConfiguration<TUser>
	where TUser : User<TUserId>
	where TUserId : struct
{
	readonly string _table;
	readonly string _schema;

	protected UserEntityTypeConfiguration(string table, string schema)
	{
		_table = Ensure.IsNotNullOrEmpty(table, nameof(table));
		_schema = Ensure.IsNotNullOrEmpty(schema, nameof(schema));
	}

	/// <inheritdoc />
	protected override void ConfigureDomain(EntityTypeBuilder<TUser> builder)
	{
		builder.ToTable(_table, _schema);

		builder.HasKey(e => e.UserId);

		builder.Property(e => e.UserId)
			.IsRequired()
			.ValueGeneratedOnAdd()
			.HasConversion<TUserIdValueConverter>();

		builder.Property(e => e.Email)
			.HasMaxLength(256)
			.IsUnicode()
			.HasConversion<EmailAddressEFValueConverter>();

		builder.Property(e => e.FormalName)
			.IsRequired()
			.HasMaxLength(512)
			.IsUnicode();

		builder.Property(e => e.Name)
			.IsRequired()
			.HasMaxLength(512)
			.IsUnicode();

		builder.Property(e => e.Handle)
			.HasMaxLength(512)
			.IsUnicode();
	}
}

public interface IUserReader<TUser, TUserId> : IReader<TUser, TUserId>
	where TUser : User<TUserId>
	where TUserId : struct
{ }

public abstract class UserReader<TUser, TUserId, TContext> : Reader<TContext, TUser, TUserId>, IUserReader<TUser, TUserId>
	where TUser : User<TUserId>
	where TUserId : struct
	where TContext : DbContext
{
	public UserReader(TContext context) : base(context) { }
}