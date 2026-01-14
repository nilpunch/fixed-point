using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Fixed32
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	[Serializable]
	public partial struct FP : IEquatable<FP>, IComparable<FP>, IFormattable
	{
		public const int FractionalBits = 16;
		public const int CalculationsEpsilonScaling = 10;

		public int RawValue;

		private FP(int rawValue)
		{
			RawValue = rawValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe FP FromRaw(int rawValue)
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
				return FromRaw((int)(((long)x << FractionalBits) / y));
			}
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
#if CHECK_OVERFLOW
			checked
#endif
			{
				x.RawValue = (int)(((long)x.RawValue * y.RawValue) >> FractionalBits);
				return x;
			}
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
#if CHECK_OVERFLOW
			checked
#endif
			{
				x.RawValue = (int)(((long)x.RawValue << FractionalBits) / y.RawValue);
				return x;
			}
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
			return x.RawValue >> FractionalBits > y;
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
			return x.RawValue >> FractionalBits < y;
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
			return x.RawValue >> FractionalBits >= y;
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
			return x.RawValue >> FractionalBits <= y;
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
