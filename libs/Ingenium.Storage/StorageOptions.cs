using FluentValidation;

namespace Ingenium.Storage;

/// <summary>
/// Represents options for configuring storage.
/// </summary>
public class StorageOptions
{
	public const string ConfigurationSectionKey = "Storage";
	public const string ProfilesConfigurationSectionKey = "Storage:Profiles";

	/// <summary>
	/// Gets or sets the default storage provider ID.
	/// </summary>
	public string? DefaultProfileId { get; set; }

	/// <summary>
	/// Gets or sets whether to enable the implicit Temp file system profile.
	/// </summary>
	public bool ImplicitTempFileSystemProfile { get; set; } = true;

	/// <summary>
	/// Gets or sets whether to enable the implicit Local file system profile.
	/// </summary>
	public bool ImplicitLocalFileSystemProfile { get; set; } = true;

	/// <summary>
	/// Gets or sets the set of storage profiles.
	/// </summary>
	public Dictionary<string, StorageProfileOptions> Profiles { get; set; } = default!;
}

/// <summary>
/// Represents options for configuring a storage profile.
/// </summary>
public class StorageProfileOptions
{
	/// <summary>
	/// Gets or sets the provider ID.
	/// </summary>
	public string ProviderId { get; set; } = default!;
}

/// <summary>
/// Validates instances of <see cref="StorageOptions"/>.
/// </summary>
public class StorageOptionsValidator : AbstractValidator<StorageOptions>
{
	public StorageOptionsValidator()
	{
		When(o => o.DefaultProfileId is { Length: > 0 }, () =>
		{
			RuleFor(o => o.DefaultProfileId).Custom((providerId, context) =>
			{
				var options = context.InstanceToValidate;
				if (options.Profiles is not { Count: >0 } 
					|| !options.Profiles.TryGetValue(providerId!, out _))
				{
					context.AddFailure($"The default storage profile '{providerId}' has not been configured.");
				}
			});
		});

		When(o => o.Profiles is { Count: > 0 }, () =>
		{
			RuleForEach(o => o.Profiles).SetDictionaryValidator(new StorageProfileOptionsValidator());
		});
	}
}

/// <summary>
/// Provides a base implementation of a storage profile options validator.
/// </summary>
/// <typeparam name="TOptions">The options type.</typeparam>
public abstract class StorageProfileOptionsValidator<TOptions> : AbstractValidator<TOptions>
	where TOptions : StorageProfileOptions
{
	public StorageProfileOptionsValidator()
	{
		RuleFor(o => o.ProviderId).NotEmpty();
	}
}

/// <summary>
/// Validates instances of <see cref="StorageProfileOptions"/>.
/// </summary>
public class StorageProfileOptionsValidator : StorageProfileOptionsValidator<StorageProfileOptions> { }