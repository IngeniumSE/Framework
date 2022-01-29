using Ingenium.Platform.Data;

namespace Ingenium.Platform.Security.Users;

/// <summary>
/// Represents a user.
/// </summary>
public class User : Entity
{
	/// <summary>
	/// Gets or sets the user ID.
	/// </summary>
	public int UserId { get; set; }

	/// <summary>
	/// Gets or sets the email.
	/// </summary>
	public string Email { get; set; } = default!;

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	public string Name { get; set; } = default!;
}
