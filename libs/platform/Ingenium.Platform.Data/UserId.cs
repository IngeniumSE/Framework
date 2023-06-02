namespace Ingenium.Platform.Security;

using static GenerateIdFeatures;

[GenerateId(
	features: EFValueConverter | SystemTextJsonConverter
)]
public partial struct UserId { }