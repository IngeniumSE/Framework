namespace Ingenium.Data;

/// <summary>
/// Provides tests for the <see cref="ConnectionStringProvider"/> type.
/// </summary>
public class ConnectionStringProviderTests
{
	[Fact]
	public void TryGetConnectionString_ReturnsTrue_AndOutputsConnectionString_ForValidName()
	{
		// Arrange
		var options = new DataOptions
		{
			ConnectionStrings = new Dictionary<string, ConnectionStringOptions>
			{
				["Default"] = new()
				{
					ConnectionString = "Server=localhost",
					ProviderId = "SqlServer"
				}
			}
		};

		// Act
		var connectionStringProvider = new ConnectionStringProvider(options);
		bool result = connectionStringProvider.TryGetConnectionString("Default", out var connectionString);

		// Assert
		Assert.True(result);
		Assert.NotNull(connectionString);
		Assert.Equal("Server=localhost", connectionString!.ConnectionString);
		Assert.Equal("SqlServer", connectionString.ProviderId.Value);
	}

	[Fact]
	public void TryGetConnectionString_ReturnsFalse_ForInvalidName()
	{
		// Arrange
		var options = new DataOptions
		{
			ConnectionStrings = new Dictionary<string, ConnectionStringOptions>
			{
				["Default"] = new()
				{
					ConnectionString = "Server=localhost",
					ProviderId = "SqlServer"
				}
			}
		};

		// Act
		var connectionStringProvider = new ConnectionStringProvider(options);
		bool result = connectionStringProvider.TryGetConnectionString("Test", out var connectionString);

		// Assert
		Assert.False(result);
		Assert.Null(connectionString);
	}

	[Fact]
	public void TryGetConnectionString_AssignsDefaultProviderId_IfNotSpecifiedExplicitly()
	{
		// Arrange
		var options = new DataOptions
		{
			DefaultProviderId = "SomeDatabase",
			ConnectionStrings = new Dictionary<string, ConnectionStringOptions>
			{
				["Default"] = new()
				{
					ConnectionString = "Server=localhost"
				}
			}
		};

		// Act
		var connectionStringProvider = new ConnectionStringProvider(options);
		bool result = connectionStringProvider.TryGetConnectionString("Default", out var connectionString);

		// Assert
		Assert.True(result);
		Assert.Equal("SomeDatabase", connectionString!.ProviderId.Value);
	}
}