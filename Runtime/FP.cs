using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Mathematics.Fixed
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	[Serializable]
	public partial struct FP : IEquatable<FP>, IComparable<FP>, IFormattable
	{
		public const int FractionalBits = 31;
		public const int CalculationsEpsilonScaling = 10;

		public long RawValue;

		private FP(long rawValue)
		{
			RawValue = rawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe FP FromRaw(long rawValue)
		{
			return *(FP*)(&rawValue);
		}

		/// <summary>
		/// Adds x and y without performing overflow checking.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator +(FP x, FP y)
		{
			var result = Add(x.RawValue, y.RawValue);
			return FromRaw(result);
		}

		/// <summary>
		/// Subtracts y from x without performing overflow checking.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator -(FP x, FP y)
		{
			var result = Sub(x.RawValue, y.RawValue);
			return FromRaw(result);
		}

		/// <summary>
		/// Performs multiplication with checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator *(FP x, FP y)
		{
			var result = Mul(x.RawValue, y.RawValue);
			return FromRaw(result);
		}

		/// <summary>
		/// Performs fast multiplication with checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator *(FP x, int scalar)
		{
			var result = MulScalar(x.RawValue, scalar);
			return FromRaw(result);
		}

		/// <summary>
		/// Performs fast multiplication with checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator *(int scalar, FP x)
		{
			return x * scalar;
		}

		/// <summary>
		/// Division by 0 returns min or max value, depending on the sign of the numerator.
		/// It has some rare minor inaccuracies, and they are tied to absolute precision.<br/>
		/// Inaccuracies are in range [0, Epsilon * 2000).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator /(FP x, FP y)
		{
			var result = Div(x.RawValue, y.RawValue);
			return FromRaw(result);
		}

		/// <summary>
		/// Performs fast division with checking for overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator /(FP x, int scalar)
		{
			var result = DivScalar(x.RawValue, scalar);
			return FromRaw(result);
		}

		/// <summary>
		/// Performs modulo as fast as possible. Throws if x == MinValue and y == -1.
		/// Use the <see cref="FMath.SafeMod"/> for a more reliable but slower modulo.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator %(FP x, FP y)
		{
			return FromRaw(x.RawValue % y.RawValue);
		}

		/// <summary>
		/// Negate x without performing overflow checking.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator -(FP x)
		{
			return FromRaw(-x.RawValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(FP x, FP y)
		{
			return x.RawValue > y.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(FP x, int y)
		{
			return x.RawValue > (long)y << FractionalBits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(int y, FP x)
		{
			return (long)y << FractionalBits > x.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(FP x, FP y)
		{
			return x.RawValue < y.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(FP x, int y)
		{
			return x.RawValue < (long)y << FractionalBits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(int y, FP x)
		{
			return (long)y << FractionalBits < x.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(FP x, FP y)
		{
			return x.RawValue >= y.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(FP x, int y)
		{
			return x.RawValue >= (long)y << FractionalBits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(int y, FP x)
		{
			return (long)y << FractionalBits >= x.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(FP x, FP y)
		{
			return x.RawValue <= y.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(FP x, int y)
		{
			return x.RawValue <= (long)y << FractionalBits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(int y, FP x)
		{
			return (long)y << FractionalBits <= x.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FP x, FP y)
		{
			return x.RawValue == y.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FP a, int b)
		{
			return a.RawValue == (long)b << FractionalBits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(int b, FP a)
		{
			return (long)b << FractionalBits == a.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FP x, FP y)
		{
			return x.RawValue != y.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FP a, int b)
		{
			return a.RawValue != (long)b << FractionalBits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(int b, FP a)
		{
			return (long)b << FractionalBits != a.RawValue;
		}

		public override bool Equals(object obj)
		{
			return obj is FP fp && fp.RawValue == RawValue;
		}

		public override int GetHashCode()
		{
			return RawValue.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FP other)
		{
			return RawValue == other.RawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CompareTo(FP other)
		{
			return RawValue.CompareTo(other.RawValue);
		}

		public string ToString(string format, IFormatProvider formatProvider) =>
			this.ToDouble().ToString(format, formatProvider);

		public string ToString(string format) => this.ToDouble().ToString(format);

		public string ToString(IFormatProvider provider) => this.ToDouble().ToString(provider);

		public override string ToString() => this.ToDouble().ToString("G", System.Globalization.CultureInfo.InvariantCulture);

		/// <summary>
		/// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Add(in FP x, in FP y)
		{
			var xl = x.RawValue;
			var yl = y.RawValue;
			var sum = xl + yl;
			// If signs of operands are equal and signs of sum and x are different
			if ((~(xl ^ yl) & (xl ^ sum) & MinValueRaw) != 0)
			{
				sum = xl > 0 ? MaxValueRaw : MinValueRaw;
			}

			return FromRaw(sum);
		}

		/// <summary>
		/// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
		/// rounds to MinValue or MaxValue depending on sign of operands.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sub(FP x, FP y)
		{
			var xl = x.RawValue;
			var yl = y.RawValue;
			var diff = xl - yl;
			// If signs of operands are different and signs of sum and x are different.
			if (((xl ^ yl) & (xl ^ diff) & MinValueRaw) != 0)
			{
				diff = xl < 0 ? MinValueRaw : MaxValueRaw;
			}

			return FromRaw(diff);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Negate(FP x)
		{
			return x.RawValue == MinValueRaw ? MaxValue : FromRaw(-x.RawValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP SafeMod(FP x, FP y)
		{
			return FromRaw(x.RawValue == MinValueRaw & y.RawValue == -1 ? 0 : x.RawValue % y.RawValue);
		}
	}
}
