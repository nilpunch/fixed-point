using System;
using System.Runtime.CompilerServices;

namespace Mathematics.Fixed
{
    public static class FixedMath
    {
        /// <summary>
        /// Returns a number indicating the sign of a Fix64 number.
        /// Returns 1 if the value is positive or 0, and -1 if it is negative.
        /// </summary>
        public static FixedPoint Sign(FixedPoint value)
        {
            return new FixedPoint(1 | (value.RawValue & FixedPoint.SignMask));
        }

        /// <summary>
        /// Returns a number indicating the sign of a Fix64 number.
        /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
        /// </summary>
        public static FixedPoint SignWithZero(FixedPoint value)
        {
            return new FixedPoint(
                value.RawValue < 0 ? -1 :
                value.RawValue > 0 ? 1 :
                0);
        }
        
        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
        /// </summary>
        public static FixedPoint Abs(FixedPoint value)
        {
            if (value.RawValue == FixedPoint.MinValueRaw)
            {
                return FixedPoint.MaxValue;
            }

            var mask = value.RawValue >> FixedPoint.BitsAmountMinusSign;
            return new FixedPoint((value.RawValue + mask) ^ mask);
        }

        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// FastAbs(Fix64.MinValue) is undefined.
        /// </summary>
        public static FixedPoint FastAbs(FixedPoint value)
        {
            var mask = value.RawValue >> FixedPoint.BitsAmountMinusSign;
            return new FixedPoint((value.RawValue + mask) ^ mask);
        }
        
        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        public static FixedPoint Floor(FixedPoint value)
        {
            // Just zero out the fractional part
            return new FixedPoint((long)((ulong)value.RawValue & 0xFFFFFFFF00000000));
        }

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified number.
        /// </summary>
        public static FixedPoint Ceiling(FixedPoint value)
        {
            var hasFractionalPart = (value.RawValue & 0x00000000FFFFFFFF) != 0;
            return hasFractionalPart ? Floor(value) + FixedPoint.One : value;
        }

        /// <summary>
        /// Rounds a value to the nearest integral value.
        /// If the value is halfway between an even and an uneven value, returns the even value.
        /// </summary>
        public static FixedPoint Round(FixedPoint value)
        {
            var fractionalPart = value.RawValue & 0x00000000FFFFFFFF;
            var integralPart = Floor(value);
            if (fractionalPart < 0x80000000)
            {
                return integralPart;
            }

            if (fractionalPart > 0x80000000)
            {
                return integralPart + FixedPoint.One;
            }

            // if number is halfway between two values, round to the nearest even number
            // this is the method used by System.Math.Round().
            return (integralPart.RawValue & FixedPoint.OneRaw) == 0
                ? integralPart
                : integralPart + FixedPoint.One;
        }

        /// <summary>
        /// Performs modulo as fast as possible; throws if x == MinValue and y == -1.
        /// Use the operator (%) for a more reliable but slower modulo.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint FastMod(FixedPoint x, FixedPoint y)
        {
            return new FixedPoint(x.RawValue % y.RawValue);
        }

        /// <summary>
        /// Adds x and y without performing overflow checking. Should be inlined by the CLR.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint FastAdd(in FixedPoint x, in FixedPoint y)
        {
            return new FixedPoint(x.RawValue + y.RawValue);
        }

        /// <summary>
        /// Subtracts y from x without performing overflow checking. Should be inlined by the CLR.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint FastSub(FixedPoint x, FixedPoint y)
        {
            return new FixedPoint(x.RawValue - y.RawValue);
        }

        /// <summary>
        /// Performs multiplication without checking for overflow.
        /// Useful for performance-critical code where the values are guaranteed not to cause overflow
        /// </summary>
        public static FixedPoint FastMul(FixedPoint x, FixedPoint y)
        {
            var xl = x.RawValue;
            var yl = y.RawValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FixedPoint.FractionalPlaces;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FixedPoint.FractionalPlaces;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FixedPoint.FractionalPlaces;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FixedPoint.FractionalPlaces;

            var sum = (long)loResult + midResult1 + midResult2 + hiResult;
            return new FixedPoint(sum);
        }
        
        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was negative.
        /// </exception>
        public static FixedPoint Sqrt(FixedPoint x)
        {
            var xl = x.RawValue;
            if (xl < 0)
            {
                // We cannot represent infinities like Single and Double, and Sqrt is
                // mathematically undefined for x < 0. So we just throw an exception.
                throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", "x");
            }

            var num = (ulong)xl;
            var result = 0UL;

            // second-to-top bit
            var bit = 1UL << (FixedPoint.BitsAmount - 2);

            while (bit > num)
            {
                bit >>= 2;
            }

            // The main part is executed twice, in order to avoid
            // using 128 bit values in computations.
            for (var i = 0; i < 2; ++i)
            {
                // First we get the top 48 bits of the answer.
                while (bit != 0)
                {
                    if (num >= result + bit)
                    {
                        num -= result + bit;
                        result = (result >> 1) + bit;
                    }
                    else
                    {
                        result = result >> 1;
                    }

                    bit >>= 2;
                }

                if (i == 0)
                {
                    // Then process it again to get the lowest 16 bits.
                    if (num > (1UL << (FixedPoint.BitsAmount / 2)) - 1)
                    {
                        // The remainder 'num' is too large to be shifted left
                        // by 32, so we have to add 1 to result manually and
                        // adjust 'num' accordingly.
                        // num = a - (result + 0.5)^2
                        //       = num + result^2 - (result + 0.5)^2
                        //       = num - result - 0.5
                        num -= result;
                        num = (num << (FixedPoint.BitsAmount / 2)) - 0x80000000UL;
                        result = (result << (FixedPoint.BitsAmount / 2)) + 0x80000000UL;
                    }
                    else
                    {
                        num <<= (FixedPoint.BitsAmount / 2);
                        result <<= (FixedPoint.BitsAmount / 2);
                    }

                    bit = 1UL << (FixedPoint.BitsAmount / 2 - 2);
                }
            }

            // Finally, if next bit would have been 1, round the result upwards.
            if (num > result)
            {
                ++result;
            }

            return new FixedPoint((long)result);
        }
        
        
        /// <summary>
        /// Returns 2 raised to the specified power.
        /// Provides at least 6 decimals of accuracy.
        /// </summary>
        internal static FixedPoint Pow2(FixedPoint x)
        {
            if (x.RawValue == 0)
            {
                return FixedPoint.One;
            }

            // Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
            bool neg = x.RawValue < 0;
            if (neg)
            {
                x = -x;
            }

            if (x == FixedPoint.One)
            {
                return neg ? FixedPoint.One / (FixedPoint)2 : (FixedPoint)2;
            }

            if (x >= FixedPoint.Log2Max)
            {
                return neg ? FixedPoint.One / FixedPoint.MaxValue : FixedPoint.MaxValue;
            }

            if (x <= FixedPoint.Log2Min)
            {
                return neg ? FixedPoint.MaxValue : FixedPoint.Zero;
            }

            /* The algorithm is based on the power series for exp(x):
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             * 
             * From term n, we get term n+1 by multiplying with x/n.
             * When the sum term drops to zero, we can stop summing.
             */

            int integerPart = (int)FixedMath.Floor(x);
            // Take fractional part of exponent
            x = new FixedPoint(x.RawValue & 0x00000000FFFFFFFF);

            var result = FixedPoint.One;
            var term = FixedPoint.One;
            int i = 1;
            while (term.RawValue != 0)
            {
                term = FixedMath.FastMul(FixedMath.FastMul(x, term), FixedPoint.Ln2) / (FixedPoint)i;
                result += term;
                i++;
            }

            result = new FixedPoint(result.RawValue << integerPart);
            if (neg)
            {
                result = FixedPoint.One / result;
            }

            return result;
        }

        /// <summary>
        /// Returns the base-2 logarithm of a specified number.
        /// Provides at least 9 decimals of accuracy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        internal static FixedPoint Log2(FixedPoint x)
        {
            if (x.RawValue <= 0)
            {
                throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");
            }

            // This implementation is based on Clay. S. Turner's fast binary logarithm
            // algorithm (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
            //     Processing Mag., pp. 124,140, Sep. 2010.)

            long b = 1U << (FixedPoint.FractionalPlaces - 1);
            long y = 0;

            long rawX = x.RawValue;
            while (rawX < FixedPoint.OneRaw)
            {
                rawX <<= 1;
                y -= FixedPoint.OneRaw;
            }

            while (rawX >= (FixedPoint.OneRaw << 1))
            {
                rawX >>= 1;
                y += FixedPoint.OneRaw;
            }

            var z = new FixedPoint(rawX);

            for (int i = 0; i < FixedPoint.FractionalPlaces; i++)
            {
                z = FixedMath.FastMul(z, z);
                if (z.RawValue >= (FixedPoint.OneRaw << 1))
                {
                    z = new FixedPoint(z.RawValue >> 1);
                    y += b;
                }

                b >>= 1;
            }

            return new FixedPoint(y);
        }

        /// <summary>
        /// Returns the natural logarithm of a specified number.
        /// Provides at least 7 decimals of accuracy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        public static FixedPoint Ln(FixedPoint x)
        {
            return FixedMath.FastMul(Log2(x), FixedPoint.Ln2);
        }

        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// Provides about 5 digits of accuracy for the result.
        /// </summary>
        /// <exception cref="DivideByZeroException">
        /// The base was zero, with a negative exponent
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The base was negative, with a non-zero exponent
        /// </exception>
        public static FixedPoint Pow(FixedPoint b, FixedPoint exp)
        {
            if (b == FixedPoint.One)
            {
                return FixedPoint.One;
            }

            if (exp.RawValue == 0)
            {
                return FixedPoint.One;
            }

            if (b.RawValue == 0)
            {
                if (exp.RawValue < 0)
                {
                    throw new DivideByZeroException();
                }

                return FixedPoint.Zero;
            }

            FixedPoint log2 = Log2(b);
            return Pow2(exp * log2);
        }
    }
}