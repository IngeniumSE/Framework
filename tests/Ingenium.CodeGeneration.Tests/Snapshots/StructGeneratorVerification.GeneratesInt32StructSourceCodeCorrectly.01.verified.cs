//HintName: TestId_IdImpl.generated.cs
namespace <global namespace>
{
	using System;
	using System.Diagnostics;

	[DebuggerDisplay("{DebuggerToString(),nq}")]
	partial struct TestId : IComparable<TestId>, IEquatable<TestId>
	{
		/// <summary>
		/// Represents an empty instance.
		/// </summary>
		public static readonly TestId Empty = new TestId();

		public TestId(Int32 value)
		{
			

			Value = value;
			HasValue = true;
		}

		/// <summary>
		/// Gets whether the instance has a value.
		/// </summary>
		public bool HasValue { get; }

		/// <summary>
		/// Gets the value.
		/// </summary>
		public Int32 Value { get; }

		/// <inheritdoc />
		public int CompareTo(TestId other)
		{
			if (HasValue && HasValue == other.HasValue)
			{
				return Value.CompareTo(other.Value);
			}

			return HasValue ? -1 : 1;
		}

		/// <inheritdoc />
		public override bool Equals(object other)
			=> other switch
				{
					TestId value => Equals(value),
					Int32 value => Equals(value),
					_ => false
				};

		/// <inheritdoc />
		public bool Equals(TestId other)
		{
			if (HasValue && HasValue == other.HasValue)
			{
				return Value.Equals(other.Value);
			}

			return false;
		}

		/// <inheritdoc />
		public bool Equals(Int32 other)
		{
			if (HasValue)
			{
				return Value.Equals(other);
			}

			return false;
		}

		/// <inheritdoc />
		public override int GetHashCode()
			=> HashCode.Combine(HasValue, Value);

		/// <inheritdoc />
		public override string ToString()
			=> HasValue ? Value.ToString() : string.Empty;

		/// <inheritdoc />
		string DebuggerToString()
			=> HasValue ? Value.ToString() : "(empty)";

		public static bool operator ==(TestId left, TestId right)
			=> left.Equals(right);
		public static bool operator !=(TestId left, TestId right)
			=> !left.Equals(right);
	}
}