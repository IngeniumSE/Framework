// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Ingenium.CodeGeneration
{
	using System;

	using Xunit;

	/// <summary>
	/// Provides tests for the <see cref="StructGenerator" /> type.
	/// </summary>
	public class StructGeneratorTests
	{
		[Fact]
		public void Generates_Int32_Backed_Struct()
		{
			// Arrange, Act
			var empty = Int32Struct.Empty;
			var ten = new Int32Struct(10);

			// Assert
			Assert.False(empty.HasValue);

			Assert.True(ten.HasValue);
			Assert.Equal(10, ten.Value);
		}

		[Fact]
		public void Int32_Backed_Struct_Should_Implement_Equatable()
		{
			// Arrange
			var empty = Int32Struct.Empty;
			var five = new Int32Struct(5);

			// Act
			bool equals = five.Equals(five);
			bool notEquals = !five.Equals(empty); ;

			// Assert
			Assert.True(equals);
			Assert.True(notEquals);
		}

		[Fact]
		public void Int32_Backed_Struct_Should_Implement_Equality_Operators()
		{
			// Arrange
			var empty = Int32Struct.Empty;
			var five = new Int32Struct(5);

			// Act
#pragma warning disable CS1718 // Comparison made to same variable
			bool equals = five == five;
#pragma warning restore CS1718 // Comparison made to same variable
			bool notEquals = five != empty;

			// Assert
			Assert.True(equals);
			Assert.True(notEquals);
		}

		[Fact]
		public void Generates_String_Backed_Struct()
		{
			// Arrange, Act
			var empty = StringStruct.Empty;
			var value = new StringStruct("value");

			// Assert
			Assert.False(empty.HasValue);
			Assert.Null(empty.Value);

			Assert.True(value.HasValue);
			Assert.Equal("value", value.Value);
		}

		[Fact]
		public void String_Backed_Struct_Constructor_ValidatesArgument()
		{
			// Arrange, Act, Assert
			Assert.Throws<ArgumentException>(() => new StringStruct(default));
			Assert.Throws<ArgumentException>(() => new StringStruct(""));
		}

		[Fact]
		public void String_Backed_Struct_Should_Implement_Equatable()
		{
			// Arrange
			var empty = StringStruct.Empty;
			var value = new StringStruct("value");

			// Act
			bool equals = value.Equals(value);
			bool notEquals = !value.Equals(empty); ;

			// Assert
			Assert.True(equals);
			Assert.True(notEquals);
		}

		[Fact]
		public void String_Backed_Struct_Should_Implement_Equatable_Compares_CaseSensitive()
		{
			// Arrange
			var value = new StringStruct("value");
			var other = new StringStruct("VALUE");

			// Act
			bool equals = value.Equals(other);

			// Assert
			Assert.False(equals);
		}

		[Fact]
		public void String_Backed_Struct_Should_Implement_Equality_Operators()
		{
			// Arrange
			var empty = StringStruct.Empty;
			var value = new StringStruct("value");

			// Act
#pragma warning disable CS1718 // Comparison made to same variable
			bool equals = value == value;
#pragma warning restore CS1718 // Comparison made to same variable
			bool notEquals = value != empty;

			// Assert
			Assert.True(equals);
			Assert.True(notEquals);
		}
	}

	[GenerateId]
	partial struct Int32Struct { }

	[GenerateId(typeof(string))]
	partial struct StringStruct { }
}
