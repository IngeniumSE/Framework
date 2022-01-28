namespace Ingenium.DependencyInjection;

/// <summary>
/// Provides tests for the <see cref="Provider{TService}"/> type.
/// </summary>
[Provider("Type")]
public class ProviderTests
{
	[Fact]
	public void ForType_ResolvesProviderAttribute_FromType_WhenDefinedExplicitly()
	{
		// Arrange, Act
		var provider = Provider<string>.ForType(typeof(ProviderTests));

		// Assert
		Assert.NotNull(provider);
		Assert.Equal("Type", provider.ProviderId.Value);
	}

	[Fact]
	public void ForType_ResolvesProviderAttribute_FromAssembly_WhenNotDefinedExplicitly()
	{
		// Arrange, Act
		var provider = Provider<string>.ForType(typeof(OtherType));

		// Assert
		Assert.NotNull(provider);
		Assert.Equal("Assembly", provider.ProviderId.Value);
	}

	[Fact]
	public void ForType_ThrowsException_WhenUnableToResolveProviderAttribute()
	{
		// Arrange, Act, Assert
		Assert.Throws<InvalidOperationException>(
			() => Provider<string>.ForType(typeof(string)));
	}

	// Does not explicitly define via the [Provider] attribute.
	class OtherType { }
}