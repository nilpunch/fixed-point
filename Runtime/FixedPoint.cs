using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
    /// <summary>
    /// Represents a Q31.32 fixed-point number.
    /// Layout: |SIGN|31|32|
    /// </summary>
    public readonly struct FixedPoint : IEquatable<FixedPoint>, IComparable<FixedPoint>
    {
        public const int BitsAmount = sizeof(long) * 8;
        public const int BitsAmountMinusSign = BitsAmount - 1;
        public const int FractionalPlaces = 32;

        public static readonly FixedPoint Epsilon = new FixedPoint(EpsilonRaw);
        public static readonly FixedPoint MaxValue = new FixedPoint(MaxValueRaw);
        public static readonly FixedPoint MinValue = new FixedPoint(MinValueRaw);
        public static readonly FixedPoint One = new FixedPoint(OneRaw);
        public static readonly FixedPoint Zero = new FixedPoint();
        public static readonly FixedPoint Pi = new FixedPoint(PiRaw);
        public static readonly FixedPoint PiOver2 = new FixedPoint(PiOver2Raw);
        public static readonly FixedPoint Log2Max = new FixedPoint(Log2MaxRaw);
        public static readonly FixedPoint Log2Min = new FixedPoint(Log2MinRaw);
        public static readonly FixedPoint Ln2 = new FixedPoint(Ln2Raw);

        public const long EpsilonRaw = 1L;
        public const long MaxValueRaw = long.MaxValue;
        public const long MinValueRaw = long.MinValue;
        public const long OneRaw = 1L << FractionalPlaces;
        public const long PiTimes2Raw = 0x6487ED511;
        public const long PiRaw = 0x3243F6A88;
        public const long PiOver2Raw = 0x1921FB544;
        public const long Ln2Raw = 0xB17217F7;
        public const long Log2MaxRaw = 0x1F00000000;
        public const long Log2MinRaw = -0x2000000000;

        public const long SignMask = 1L << BitsAmountMinusSign;

        public readonly long RawValue;

        /// <summary>
        /// This is the constructor from raw value.
        /// </summary>
        /// <param name="rawValue"></param>
        public FixedPoint(long rawValue)
        {
            RawValue = rawValue;
        }

        /// <summary>
        /// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        public static FixedPoint operator +(FixedPoint x, FixedPoint y)
        {
            var xl = x.RawValue;
            var yl = y.RawValue;
            var sum = xl + yl;
            // if signs of operands are equal and signs of sum and x are different
            if (((~(xl ^ yl) & (xl ^ sum)) & MinValueRaw) != 0)
            {
                sum = xl > 0 ? MaxValueRaw : MinValueRaw;
            }

            return new FixedPoint(sum);
        }

        /// <summary>
        /// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        public static FixedPoint operator -(FixedPoint x, FixedPoint y)
        {
            var xl = x.RawValue;
            var yl = y.RawValue;
            var diff = xl - yl;
            // if signs of operands are different and signs of sum and x are different
            if ((((xl ^ yl) & (xl ^ diff)) & MinValueRaw) != 0)
            {
                diff = xl < 0 ? MinValueRaw : MaxValueRaw;
            }

            return new FixedPoint(diff);
        }

        private static long AddOverflowHelper(long x, long y, ref bool overflow)
        {
            var sum = x + y;
            // x + y overflows if sign(x) ^ sign(y) != sign(sum)
            overflow |= ((x ^ y ^ sum) & MinValueRaw) != 0;
            return sum;
        }

        public static FixedPoint operator *(FixedPoint x, FixedPoint y)
        {
            var xl = x.RawValue;
            var yl = y.RawValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FractionalPlaces;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FractionalPlaces;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FractionalPlaces;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FractionalPlaces;

            bool overflow = false;
            var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
            sum = AddOverflowHelper(sum, midResult2, ref overflow);
            sum = AddOverflowHelper(sum, hiResult, ref overflow);

            bool opSignsEqual = ((xl ^ yl) & MinValueRaw) == 0;

            // if signs of operands are equal and sign of result is negative,
            // then multiplication overflowed positively
            // the reverse is also true
            if (opSignsEqual)
            {
                if (sum < 0 || (overflow && xl > 0))
                {
                    return MaxValue;
                }
            }
            else
            {
                if (sum > 0)
                {
                    return MinValue;
                }
            }

            // if the top 32 bits of hihi (unused in the result) are neither all 0s or 1s,
            // then this means the result overflowed.
            var topCarry = hihi >> FractionalPlaces;
            if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/)
            {
                return opSignsEqual ? MaxValue : MinValue;
            }

            // If signs differ, both operands' magnitudes are greater than 1,
            // and the result is greater than the negative operand, then there was negative overflow.
            if (!opSignsEqual)
            {
                long posOp, negOp;
                if (xl > yl)
                {
                    posOp = xl;
                    negOp = yl;
                }
                else
                {
                    posOp = yl;
                    negOp = xl;
                }

                if (sum > negOp && negOp < -OneRaw && posOp > OneRaw)
                {
                    return MinValue;
                }
            }

            return new FixedPoint(sum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountLeadingZeroes(ulong x)
        {
            int result = 0;
            while ((x & 0xF000000000000000) == 0)
            {
                result += 4;
                x <<= 4;
            }

            while ((x & 0x8000000000000000) == 0)
            {
                result += 1;
                x <<= 1;
            }

            return result;
        }

        public static FixedPoint operator /(FixedPoint x, FixedPoint y)
        {
            var xl = x.RawValue;
            var yl = y.RawValue;

            if (yl == 0)
            {
                throw new DivideByZeroException();
            }

            var remainder = (ulong)(xl >= 0 ? xl : -xl);
            var divider = (ulong)(yl >= 0 ? yl : -yl);
            var quotient = 0UL;
            var bitPos = BitsAmount / 2 + 1;


            // If the divider is divisible by 2^n, take advantage of it.
            while ((divider & 0xF) == 0 && bitPos >= 4)
            {
                divider >>= 4;
                bitPos -= 4;
            }

            while (remainder != 0 && bitPos >= 0)
            {
                int shift = CountLeadingZeroes(remainder);
                if (shift > bitPos)
                {
                    shift = bitPos;
                }

                remainder <<= shift;
                bitPos -= shift;

                var div = remainder / divider;
                remainder = remainder % divider;
                quotient += div << bitPos;

                // Detect overflow
                if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0)
                {
                    return ((xl ^ yl) & MinValueRaw) == 0 ? MaxValue : MinValue;
                }

                remainder <<= 1;
                --bitPos;
            }

            // rounding
            ++quotient;
            var result = (long)(quotient >> 1);
            if (((xl ^ yl) & MinValueRaw) != 0)
            {
                result = -result;
            }

            return new FixedPoint(result);
        }

        public static FixedPoint operator %(FixedPoint x, FixedPoint y)
        {
            return new FixedPoint(
                x.RawValue == MinValueRaw & y.RawValue == -1 ? 0 : x.RawValue % y.RawValue);
        }

        public static FixedPoint operator -(FixedPoint x)
        {
            return x.RawValue == MinValueRaw ? MaxValue : new FixedPoint(-x.RawValue);
        }

        public static bool operator ==(FixedPoint x, FixedPoint y)
        {
            return x.RawValue == y.RawValue;
        }

        public static bool operator !=(FixedPoint x, FixedPoint y)
        {
            return x.RawValue != y.RawValue;
        }

        public static bool operator >(FixedPoint x, FixedPoint y)
        {
            return x.RawValue > y.RawValue;
        }

        public static bool operator <(FixedPoint x, FixedPoint y)
        {
            return x.RawValue < y.RawValue;
        }

        public static bool operator >=(FixedPoint x, FixedPoint y)
        {
            return x.RawValue >= y.RawValue;
        }

        public static bool operator <=(FixedPoint x, FixedPoint y)
        {
            return x.RawValue <= y.RawValue;
        }

        public static explicit operator FixedPoint(long value)
        {
            return new FixedPoint(value * OneRaw);
        }

        public static explicit operator long(FixedPoint value)
        {
            return value.RawValue >> FractionalPlaces;
        }

        public static explicit operator FixedPoint(float value)
        {
            return new FixedPoint((long)(value * OneRaw));
        }

        public static explicit operator float(FixedPoint value)
        {
            return (float)value.RawValue / OneRaw;
        }

        public static explicit operator FixedPoint(double value)
        {
            return new FixedPoint((long)(value * OneRaw));
        }

        public static explicit operator double(FixedPoint value)
        {
            return (double)value.RawValue / OneRaw;
        }

        public static explicit operator FixedPoint(decimal value)
        {
            return new FixedPoint((long)(value * OneRaw));
        }

        public static explicit operator decimal(FixedPoint value)
        {
            return (decimal)value.RawValue / OneRaw;
        }

        public override bool Equals(object obj)
        {
            return obj is FixedPoint && ((FixedPoint)obj).RawValue == RawValue;
        }

        public override int GetHashCode()
        {
            return RawValue.GetHashCode();
        }

        public bool Equals(FixedPoint other)
        {
            return RawValue == other.RawValue;
        }

        public int CompareTo(FixedPoint other)
        {
            return RawValue.CompareTo(other.RawValue);
        }

        public override string ToString()
        {
            // Up to 10 decimal places
            return ((decimal)this).ToString("0.##########");
        }
    }
}