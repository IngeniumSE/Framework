namespace Ingenium
{
	/// <summary>
	/// Marks a struct as a candidate for code generation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
	public class GenerateIdAttribute : Attribute
	{
		/// <summary>
		/// Initialises a new instance of <see cref="GenerateIdAttribute"/>
		/// </summary>
		/// <param name="backingType">The backing type.</param>
		/// <param name="caseInsensitive">Should a string backing field be case insensitive.</param>
		/// <param name="features">Determines the features to be generated.</param>
		/// <exception cref="ArgumentNullException">If the backing type is null.</exception>
		public GenerateIdAttribute(
			Type? backingType = default,
			bool caseInsensitive = false,
			GenerateIdFeatures features = GenerateIdFeatures.None)
		{
			BackingType = backingType ?? typeof(int);
			CaseInsensitive = caseInsensitive;
			Features = features;
		}


		/// <summary>
		/// Gets the backing type.
		/// </summary>
		public Type BackingType { get; }

		/// <summary>
		/// Gets or sets whether a string backing field should be case insensitive.
		/// </summary>
		public bool CaseInsensitive { get; }

		/// <summary>
		/// Gets the features to generate for the target type.
		/// </summary>
		public GenerateIdFeatures Features { get; }
	}

	/// <summary>
	/// Represents the supported features to generate for the ID.
	/// </summary>
	[Flags]
	public enum GenerateIdFeatures
	{
		None = 0,
		EFValueConverter = 1,
		SystemTextJsonConverter = 2,
		AspNetCoreModelBinder = 4,
		TypeConverter = 8
	}
}