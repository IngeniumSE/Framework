using System.Text;

using Microsoft.CodeAnalysis;

namespace Ingenium.CodeGeneration
{
	static class SourceGenerationHelper
	{
		public const string AttributeFullName = "Ingenium.GenerateIdAttribute";

		public static string GenerateIdClass(IdGenerator.StructToGenerate target)
		{
			var builder = new StringBuilder();

			builder.AppendLine(GeneratePartialStruct(target));

			if (target.Features.HasFlag(GenerateIdFeatures.EFValueConverter))
			{
				builder.AppendLine(GenerateEFValueConverter(target));
			}

			if (target.Features.HasFlag(GenerateIdFeatures.SystemTextJsonConverter))
			{
				builder.AppendLine(GenerateSTJJsonConverter(target));
			}

			if (target.Features.HasFlag(GenerateIdFeatures.AspNetCoreModelBinder))
			{
				builder.AppendLine(GenerateAspNetCoreModelBinder(target));
			}

			return builder.ToString();
		}

		static string GeneratePartialStruct(IdGenerator.StructToGenerate target)
		{
			string typeName = target.Type.Name;
			string typeNameSpace = target.Type.ContainingNamespace.ToDisplayString();
			string backingTypeName = target.BackingType.ToDisplayString();

			var stringType = target.Compilation.GetTypeByMetadataName("System.String");
			var valueType = target.Compilation.GetTypeByMetadataName("System.ValueType");

			bool isString = SymbolEqualityComparer.Default.Equals(stringType, target.BackingType);
			bool isValueType = target.BackingType.BaseType is not null && SymbolEqualityComparer.Default.Equals(valueType, target.BackingType.BaseType);

			bool hasSystemTextJsonConverter = target.Features.HasFlag(GenerateIdFeatures.SystemTextJsonConverter);
			bool hasAspNetCoreModelBinder = target.Features.HasFlag(GenerateIdFeatures.AspNetCoreModelBinder);
			bool hasTypeConverter = target.Features.HasFlag(GenerateIdFeatures.TypeConverter);

			return $@"namespace {typeNameSpace}
{{
	using System;
	using System.Diagnostics;

	[DebuggerDisplay(""{{DebuggerToString(){(isString ? "" : ",nq")}}}"")]
	{(hasSystemTextJsonConverter ? $"[System.Text.Json.Serialization.JsonConverter(typeof({typeName}STJsonConverter))]" : "")}
	{(hasAspNetCoreModelBinder ? $"[Microsoft.AspNetCore.Mvc.ModelBinder(BinderType = typeof({typeName}AspNetCoreModelBinder))]" : "")}
	{(hasTypeConverter ? $"[System.ComponentModel.TypeConverter(typeof({typeName}TypeConverter))]" : "")}
	partial struct {typeName} : IComparable<{typeName}>, IEquatable<{typeName}>
	{{
		public static readonly IEqualityComparer<{typeName}> Comparer = new _Comparer();

		/// <summary>
		/// Represents an empty instance.
		/// </summary>
		public static readonly {typeName} Empty = new {typeName}();

		public {typeName}({backingTypeName} value)
		{{
			{GenerateArgumentValidation(isValueType, isString)}

			Value = value;
			HasValue = true;
		}}

		/// <summary>
		/// Gets whether the instance has a value.
		/// </summary>
		public bool HasValue {{ get; }}

		/// <summary>
		/// Gets the value.
		/// </summary>
		public {backingTypeName} Value {{ get; }}

		/// <inheritdoc />
		public int CompareTo({typeName} other)
		{{
			if (HasValue && HasValue == other.HasValue)
			{{
				return Value.CompareTo(other.Value);
			}}

			return HasValue ? -1 : 1;
		}}

		/// <inheritdoc />
		public override bool Equals(object other)
			=> other switch
				{{
					{typeName} value => Equals(value),
					{backingTypeName} value => Equals(value),
					_ => false
				}};

		/// <inheritdoc />
		public bool Equals({typeName} other)
		{{
			if (HasValue && HasValue == other.HasValue)
			{{
				return {GenerateEqualityCheck(isString, "other.Value", target.CaseInsensitive)};
			}}

			return false;
		}}

		/// <inheritdoc />
		public bool Equals({backingTypeName} other)
		{{
			if (HasValue)
			{{
				return {GenerateEqualityCheck(isString, "other", target.CaseInsensitive)};
			}}

			return false;
		}}

		/// <inheritdoc />
		public override int GetHashCode()
			=> HashCode.Combine(HasValue, Value);

		/// <inheritdoc />
		public override string ToString()
			=> HasValue ? Value.ToString() : string.Empty;

		/// <inheritdoc />
		string DebuggerToString()
			=> HasValue ? Value.ToString() : ""(empty)"";

		public static bool operator ==({typeName} left, {typeName} right)
			=> left.Equals(right);
		public static bool operator !=({typeName} left, {typeName} right)
			=> !left.Equals(right);

		class _Comparer : IEqualityComparer<{typeName}>
		{{
			public bool Equals({typeName} left, {typeName} right)
			{{
				return left.Equals(right);
			}}

			public int GetHashCode({typeName} value)
			{{
				return value.GetHashCode();
			}}
		}}
	}}
}}";
		}

		static string GenerateEFValueConverter(IdGenerator.StructToGenerate target)
		{
			var dbContextType = target.Compilation.GetTypeByMetadataName("Microsoft.EntityFrameworkCore.DbContext");
			if (dbContextType is null)
			{
				throw new InvalidOperationException($"To generate an Entity Framework Core value converter, requires a reference to Entity Framework Core");
			}

			string typeName = target.Type.Name;
			string typeNameSpace = target.Type.ContainingNamespace.ToDisplayString();
			string backingTypeName = target.BackingType.ToDisplayString();

			return $@"
namespace {typeNameSpace}
{{
	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	public class {typeName}EFValueConverter : ValueConverter<{typeName}, {backingTypeName}>
	{{
		public {typeName}EFValueConverter() : base(v => v.Value, v => new {typeName}(v)) {{ }}
	}}
}}";
		}

		static string GenerateSTJJsonConverter(IdGenerator.StructToGenerate target)
		{
			var anchorType = target.Compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonConverter");
			if (anchorType is null)
			{
				throw new InvalidOperationException($"To generate an System.Text.Json json converter, requires a reference to System.Text.Json");
			}

			string typeName = target.Type.Name;
			string typeNameSpace = target.Type.ContainingNamespace.ToDisplayString();
			string backingTypeName = target.BackingType.Name;

			var stringType = target.Compilation.GetTypeByMetadataName("System.String");
			bool isString = SymbolEqualityComparer.Default.Equals(stringType, target.BackingType);

			return $@"
namespace {typeNameSpace}
{{
	using System.Text.Json;
	using System.Text.Json.Serialization;

	public class {typeName}STJsonConverter : JsonConverter<{typeName}>
	{{
		public override bool HandleNull => true;

		public override {typeName} Read(
			ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options)
		{{
			if (reader.TokenType == JsonTokenType.Null)
			{{
				return {typeName}.Empty;
			}}

			var value = reader.Get{backingTypeName}();

			return new {typeName}(value);
		}}

		public override void Write(
			Utf8JsonWriter writer,
			{typeName} value,
			JsonSerializerOptions options)
		{{
			if (value.HasValue)
			{{
				writer.Write{(isString ? "String" : "Number")}Value(value.Value);
			}}
			else
			{{
				writer.WriteNullValue();
			}}
		}}
	}}
}}";
		}

		static string GenerateAspNetCoreModelBinder(IdGenerator.StructToGenerate target)
		{
			var anchorType = target.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.IModelBinder");
			if (anchorType is null)
			{
				throw new InvalidOperationException($"To generate an ASP.NET Core MVC model binder, requires a reference to Microsoft.AspNetCore.Mvc");
			}

			string typeName = target.Type.Name;
			string typeNameSpace = target.Type.ContainingNamespace.ToDisplayString();
			string backingTypeName = target.BackingType.ToDisplayString();

			var stringType = target.Compilation.GetTypeByMetadataName("System.String");
			bool isString = SymbolEqualityComparer.Default.Equals(stringType, target.BackingType);

			return $@"
namespace {typeNameSpace}
{{
	using System;
	using System.ComponentModel;
	using System.Threading;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Mvc;

	public class {typeName}ModelBinder : IModelBinder
	{{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{{
			var modelName = bindingContext.ModelName;

			var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
			if (valueProviderResult == ValueProviderResult.None)
			{{
				return Task.CompletedTask;
			}}

			bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

			var value = valueProviderResult.FirstValue;
			if (string.IsNullOrEmpty(value))
			{{
				return Task.CompletedTask;
			}}

			var convertedValue = {(isString ? "value" : $"({backingTypeName})TypeCoverter.GetConverter(typeof({backingTypeName})).ConvertFromString(value)")};
			bindingContext.Result = ModelBindingResult.Success(new {typeName}(convertedValue));
			return Task.CompletedTas;
		}}
	}}
}}";
		}

		static string GenerateArgumentValidation(bool isValueType, bool isStringType)
			=> isValueType
					? string.Empty
					: isStringType
						? $@"if (value is not {{ Length: >0 }}) {{ throw new ArgumentException(""The provided value must be a non-empty string."", nameof(value)); }}"
						: $@"if (value is null) {{ throw new ArgumentNullException(nameof(value)); }}";

		static string GenerateEqualityCheck(bool isStringType, string valueSymbol, bool caseInsensitive)
			=> isStringType
					? $"Value.Equals({valueSymbol}, StringComparison.Ordinal{(caseInsensitive ? "IgnoreCase" : "")})"
					: $"Value.Equals({valueSymbol})";
	}
}
