using System.Transactions;

using FluentValidation;

namespace Ingenium.Data;

/// <summary>
/// Represents default options for configuring the Data module.
/// </summary>
public class DataOptions
{
	public const string ConfigurationSectionKey = "Data";

	/// <summary>
	/// Gets or sets the default isolation level for transactions.
	/// </summary>
	public IsolationLevel DefaultTransactionIsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

	/// <summary>
	/// Gets or sets the default timeout for transactions.
	/// </summary>
	public TimeSpan DefaultTransactionTimeout { get; set; } = TimeSpan.FromMinutes(1);

	/// <summary>
	/// Gets or sets the default provider ID.
	/// </summary>
	public string DefaultProviderId { get; set; } = "Data.SqlServer";

	/// <summary>
	/// Gets the set of configured connection strings.
	/// </summary>
	public Dictionary<string, ConnectionStringOptions>? ConnectionStrings { get; set; }
}

/// <summary>
/// Represents a connection string.
/// </summary>
public class ConnectionStringOptions
{
	/// <summary>
	/// Gets the provider ID.
	/// </summary>
	public string? ProviderId { get; set; }

	/// <summary>
	/// Gets the connection string.
	/// </summary>
	public string ConnectionString { get; set; } = default!;
}

/// <summary>
/// Validates instances of <see cref="DataOptions"/>.
/// </summary>
public class DataOptionsValidator : AbstractValidator<DataOptions>
{
	public DataOptionsValidator()
	{
		RuleFor(o => o.DefaultProviderId).NotEmpty();
		RuleFor(o => o.DefaultTransactionTimeout).NotEmpty();
		RuleFor(o => o.DefaultTransactionIsolationLevel).IsInEnum();

		When(o => o.ConnectionStrings is { Count: >0 }, () =>
		{
			RuleForEach(o => o.ConnectionStrings)
				.SetDictionaryValidator(
					new ConnectionStringOptionsValidator()
				);
		});
	}
}

/// <summary>
/// Validates instances of <see cref="ConnectionStringOptions"/>.
/// </summary>
public class ConnectionStringOptionsValidator : AbstractValidator<ConnectionStringOptions>
{
	public ConnectionStringOptionsValidator()
	{
		RuleFor(o => o.ConnectionString).NotEmpty();
	}
}