using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Ingenium.Platform.Security;

namespace Ingenium.Platform.Data;

/// <summary>
/// Represents an entity.
/// </summary>
public abstract class Entity
{
	/// <summary>
	/// Gets or sets the date/time the entity was created.
	/// </summary>
	public DateTimeOffset Created { get; set; }

	/// <summary>
	/// Gets or sets the ID of the user that created the entity.
	/// </summary>
	public UserId CreatedUserId { get; set; }

	/// <summary>
	/// Gets or sets whether the entity is deleted.
	/// </summary>
	public bool IsDeleted { get; set; }

	/// <summary>
	/// Gets or sets whether the entity is enabled.
	/// </summary>
	public bool IsEnabled { get; set; }

	/// <summary>
	/// Gets or sets whether the entity is hidden.
	/// </summary>
	public bool IsHidden { get; set; }

	/// <summary>
	/// Gets or sets whether the entity is locked.
	/// </summary>
	public bool IsLocked { get; set; }

	/// <summary>
	/// Gets or sets the date/time the entity was updated.
	/// </summary>
	public DateTimeOffset? Updated { get; set; }

	/// <summary>
	/// Gets or sets the ID of the user that last updated the entity.
	/// </summary>
	public UserId? UpdatedUserId { get; set; }
}

/// <summary>
/// Provides a base implementation of an entity type configuration.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public abstract class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
	where TEntity : Entity
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<TEntity> builder)
	{
		ConfigureEntity(builder);
		ConfigureDomain(builder);
	}

	/// <summary>
	/// Configures the common entity properties for this entity.
	/// </summary>
	/// <param name="builder">The entity builder.</param>
	protected virtual void ConfigureEntity(EntityTypeBuilder<TEntity> builder)
	{
		builder.Property(e => e.Created)
			.IsRequired()
			.ValueGeneratedOnAdd();

		builder.Property(e => e.CreatedUserId)
			.IsRequired()
			.HasConversion<UserIdEFValueConverter>();

		builder.Property(e => e.IsDeleted).IsRequired();
		builder.Property(e => e.IsEnabled).IsRequired();
		builder.Property(e => e.IsHidden).IsRequired();
		builder.Property(e => e.IsLocked).IsRequired();
		builder.Property(e => e.UpdatedUserId).HasConversion<UserIdEFValueConverter>();
		builder.Property(e => e.Updated);
	}

	/// <summary>
	/// Configures the domain-specific properties for this entity.
	/// </summary>
	/// <param name="builder">The entity builder.</param>
	protected abstract void ConfigureDomain(EntityTypeBuilder<TEntity> builder);
}