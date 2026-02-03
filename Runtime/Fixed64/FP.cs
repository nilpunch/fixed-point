using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Fixed64
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	[Il2CppEagerStaticClassConstruction]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP FromRatio(int x, int y)
		{
#if CHECK_OVERFLOW
			checked
#endif
			{
				return FromRaw(((long)x << FractionalBits) / y);
			}
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Add(FP x, FP y)
		{
			return FromRaw(Add(x.RawValue, y.RawValue));
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Sub(FP x, FP y)
		{
			return FromRaw(Sub(x.RawValue, y.RawValue));
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Neg(FP x)
		{
			return FromRaw(Neg(x.RawValue));
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP MulScalar(FP x, int scalar)
		{
			return FromRaw(MulScalar(x.RawValue, scalar));
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP DivScalar(FP x, int scalar)
		{
			return FromRaw(DivScalar(x.RawValue, scalar));
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Mul(FP x, FP y)
		{
			return FromRaw(Mul(x.RawValue, y.RawValue));
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Mod(FP x, FP y)
		{
			return FromRaw(Mod(x.RawValue, y.RawValue));
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Div(FP x, FP y)
		{
			return FromRaw(Div(x.RawValue, y.RawValue));
		}

		/// <summary>
		/// Operation with saturation to <see cref="FP.MinValueRaw"/> or <see cref="FP.MaxValueRaw"/> in case of overflow.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP DivRem(FP x, FP y, out FP remainder)
		{
			return FromRaw(DivRem(x.RawValue, y.RawValue, out remainder.RawValue));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator +(FP x, FP y)
		{
#if CHECK_OVERFLOW
			checked
#endif
			{
				x.RawValue += y.RawValue;
				return x;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator -(FP x, FP y)
		{
#if CHECK_OVERFLOW
			checked
#endif
			{
				x.RawValue -= y.RawValue;
				return x;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator *(FP x, FP y)
		{
			var result = Mul(x.RawValue, y.RawValue);
			return FromRaw(result);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator *(FP x, int scalar)
		{
#if CHECK_OVERFLOW
			checked
#endif
			{
				x.RawValue *= scalar;
				return x;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator *(int scalar, FP x)
		{
#if CHECK_OVERFLOW
			checked
#endif
			{
				x.RawValue *= scalar;
				return x;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator /(FP x, FP y)
		{
			var result = Div(x.RawValue, y.RawValue);
			return FromRaw(result);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator /(FP x, int scalar)
		{
			x.RawValue /= scalar;
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator %(FP x, FP y)
		{
#if CHECK_OVERFLOW
			checked
#endif
			{
				x.RawValue %= y.RawValue;
				return x;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP operator -(FP x)
		{
#if CHECK_OVERFLOW
			checked
#endif
			{
				x.RawValue = -x.RawValue;
				return x;
			}
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
	}
}
