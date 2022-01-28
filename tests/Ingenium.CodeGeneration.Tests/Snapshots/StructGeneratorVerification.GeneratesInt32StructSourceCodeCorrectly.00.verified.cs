//HintName: GenerateIdAttribute.cs

namespace Ingenium.CodeGeneration
{
	using System;

	/// <summary>
	/// Marks a struct for code generation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	sealed class GenerateIdAttribute : Attribute
	{
		/// <summary>
		/// Initialises a new instance of <see cref="Alium.CodeGeneration.GenerateIdAttribute" />
		/// <summary>
		/// <param name="backingType">The backing type of the struct.</param>
		public GenerateIdAttribute(Type? backingType = default)
		{
			BackingType = backingType is object ? backingType : typeof(int);
		}

		/// <summary>
		/// Gets the backing type for the struct.
		/// </summary>
		public Type BackingType { get; }
	}
}