using System.Net.Mail;

namespace Ingenium.Platform.Data;

using static GenerateIdFeatures;

/// <summary>
/// Represents an email address.
/// </summary>
[GenerateId(
	typeof(string), 
	caseInsensitive: true,
	features: EFValueConverter | SystemTextJsonConverter
)]
public partial struct EmailAddress
{
	/// <summary>
	/// Validates that the email address is valid.
	/// </summary>
	/// <returns>True if the email address is valid, otherwise false.</returns>
	public bool IsValid()
		=> MailAddress.TryCreate(Value, out _);
}
