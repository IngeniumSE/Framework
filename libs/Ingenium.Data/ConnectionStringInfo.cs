namespace Ingenium.Data;

/// <summary>
/// Represents information about a connection string.
/// </summary>
/// <param name="Name">The connection string name.</param>
/// <param name="ConnectionString">The connection string.</param>
/// <param name="ProviderId">The provider ID.</param>
public record ConnectionStringInfo(
	string Name,
	string ConnectionString,
	ProviderId ProviderId);