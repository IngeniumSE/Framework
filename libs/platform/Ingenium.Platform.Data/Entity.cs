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
	public int CreatedUserId { get; set; }

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
	/// Gets or sets the ID o fthe user that last updated the entity.
	/// </summary>
	public int? UpdatedUserId { get; set; }
}
