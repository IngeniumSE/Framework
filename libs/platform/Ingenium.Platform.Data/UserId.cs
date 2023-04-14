namespace Ingenium.Platform.Data;

using static GenerateIdFeatures;

[GenerateId(
	features: EFValueConverter | SystemTextJsonConverter
)]
public partial struct UserId { }