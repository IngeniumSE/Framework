using System.Data;

namespace Ingenium.Data;

/// <summary>
/// Represents command settings used to configure a SQL command.
/// </summary>
/// <param name="CommandType">The command type.</param>
/// <param name="Parameters">The command parameters.</param>
/// <param name="Timeout">The command timeout.</param>
/// <param name="Buffered">Flag which enables buffering on the command.</param>
/// <param name="CancellationToken">The cancellation token.</param>
public record struct CommandSettings(
	CommandType CommandType = CommandType.StoredProcedure,
	object? Parameters = default,
	int? Timeout = default,
	bool Buffered = true,
	CancellationToken CancellationToken = default)
{
	public static CommandSettings Default = new();
}