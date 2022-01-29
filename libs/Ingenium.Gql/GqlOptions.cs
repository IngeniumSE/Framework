using FluentValidation;

namespace Ingenium.Gql;

/// <summary>
/// Represents GraphQL options.
/// </summary>
public class GqlOptions
{
	public const string ConfigurationSectionKey = "GraphQL";

	/// <summary>
	/// Gets or sets the root query name.
	/// </summary>
	public string RootQueryName { get; set; } = "Root";
}

/// <summary>
/// Validates instances of <see cref="GqlOptions"/>.
/// </summary>
public class GqlOptionsValidator : AbstractValidator<GqlOptions>
{
	public GqlOptionsValidator()
	{
		RuleFor(o => o.RootQueryName).NotEmpty();
	}
}