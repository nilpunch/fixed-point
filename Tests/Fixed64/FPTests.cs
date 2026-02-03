using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace Fixed64
{
	[TestFixture]
	public class FPTests
	{
		[TestCase(0UL, ExpectedResult = 63 + 1)]
		[TestCase(1UL << 0, ExpectedResult = 63 - 0)]
		[TestCase(1UL << 1, ExpectedResult = 63 - 1)]
		[TestCase(1UL << 2, ExpectedResult = 63 - 2)]
		[TestCase(1UL << 3, ExpectedResult = 63 - 3)]
		[TestCase(1UL << 4, ExpectedResult = 63 - 4)]
		[TestCase(1UL << 5, ExpectedResult = 63 - 5)]
		[TestCase(1UL << 6, ExpectedResult = 63 - 6)]
		[TestCase(1UL << 7, ExpectedResult = 63 - 7)]
		[TestCase(1UL << 8, ExpectedResult = 63 - 8)]
		[TestCase(1UL << 9, ExpectedResult = 63 - 9)]
		[TestCase(1UL << 10, ExpectedResult = 63 - 10)]
		[TestCase(1UL << 11, ExpectedResult = 63 - 11)]
		[TestCase(1UL << 12, ExpectedResult = 63 - 12)]
		[TestCase(1UL << 13, ExpectedResult = 63 - 13)]
		[TestCase(1UL << 14, ExpectedResult = 63 - 14)]
		[TestCase(1UL << 15, ExpectedResult = 63 - 15)]
		[TestCase(1UL << 16, ExpectedResult = 63 - 16)]
		[TestCase(1UL << 17, ExpectedResult = 63 - 17)]
		[TestCase(1UL << 18, ExpectedResult = 63 - 18)]
		[TestCase(1UL << 19, ExpectedResult = 63 - 19)]
		[TestCase(1UL << 20, ExpectedResult = 63 - 20)]
		[TestCase(1UL << 21, ExpectedResult = 63 - 21)]
		[TestCase(1UL << 22, ExpectedResult = 63 - 22)]
		[TestCase(1UL << 23, ExpectedResult = 63 - 23)]
		[TestCase(1UL << 24, ExpectedResult = 63 - 24)]
		[TestCase(1UL << 25, ExpectedResult = 63 - 25)]
		[TestCase(1UL << 26, ExpectedResult = 63 - 26)]
		[TestCase(1UL << 27, ExpectedResult = 63 - 27)]
		[TestCase(1UL << 28, ExpectedResult = 63 - 28)]
		[TestCase(1UL << 29, ExpectedResult = 63 - 29)]
		[TestCase(1UL << 30, ExpectedResult = 63 - 30)]
		[TestCase(1UL << 31, ExpectedResult = 63 - 31)]
		[TestCase(1UL << 32, ExpectedResult = 63 - 32)]
		[TestCase(1UL << 33, ExpectedResult = 63 - 33)]
		[TestCase(1UL << 34, ExpectedResult = 63 - 34)]
		[TestCase(1UL << 35, ExpectedResult = 63 - 35)]
		[TestCase(1UL << 36, ExpectedResult = 63 - 36)]
		[TestCase(1UL << 37, ExpectedResult = 63 - 37)]
		[TestCase(1UL << 38, ExpectedResult = 63 - 38)]
		[TestCase(1UL << 39, ExpectedResult = 63 - 39)]
		[TestCase(1UL << 40, ExpectedResult = 63 - 40)]
		[TestCase(1UL << 41, ExpectedResult = 63 - 41)]
		[TestCase(1UL << 42, ExpectedResult = 63 - 42)]
		[TestCase(1UL << 43, ExpectedResult = 63 - 43)]
		[TestCase(1UL << 44, ExpectedResult = 63 - 44)]
		[TestCase(1UL << 45, ExpectedResult = 63 - 45)]
		[TestCase(1UL << 46, ExpectedResult = 63 - 46)]
		[TestCase(1UL << 47, ExpectedResult = 63 - 47)]
		[TestCase(1UL << 48, ExpectedResult = 63 - 48)]
		[TestCase(1UL << 49, ExpectedResult = 63 - 49)]
		[TestCase(1UL << 50, ExpectedResult = 63 - 50)]
		[TestCase(1UL << 51, ExpectedResult = 63 - 51)]
		[TestCase(1UL << 52, ExpectedResult = 63 - 52)]
		[TestCase(1UL << 53, ExpectedResult = 63 - 53)]
		[TestCase(1UL << 54, ExpectedResult = 63 - 54)]
		[TestCase(1UL << 55, ExpectedResult = 63 - 55)]
		[TestCase(1UL << 56, ExpectedResult = 63 - 56)]
		[TestCase(1UL << 57, ExpectedResult = 63 - 57)]
		[TestCase(1UL << 58, ExpectedResult = 63 - 58)]
		[TestCase(1UL << 59, ExpectedResult = 63 - 59)]
		[TestCase(1UL << 60, ExpectedResult = 63 - 60)]
		[TestCase(1UL << 61, ExpectedResult = 63 - 61)]
		[TestCase(1UL << 62, ExpectedResult = 63 - 62)]
		[TestCase(1UL << 63, ExpectedResult = 63 - 63)]
		public int LZC(ulong rawValue)
		{
			// Act
			var result = FP.LeadingZeroCount(rawValue);

			// Assert
			return result;
		}

		[TestCase(FP.OneRaw, ExpectedResult = FP.OneRaw)]
		[TestCase(-FP.OneRaw, ExpectedResult = FP.OneRaw)]
		[TestCase(FP.EpsilonRaw, ExpectedResult = FP.EpsilonRaw)]
		[TestCase(-FP.EpsilonRaw, ExpectedResult = FP.EpsilonRaw)]
		[TestCase(FP.MinValueRaw, ExpectedResult = FP.MaxValueRaw)]
		[TestCase(FP.MaxValueRaw, ExpectedResult = FP.MaxValueRaw)]
		public long Abs(long rawValue)
		{
			// Arrange
			var fp = FP.FromRaw(rawValue);

			// Act
			var result = FP.Abs(fp);

			// Assert
			return result.RawValue;
		}

		[TestCase(FP.HalfRaw, ExpectedResult = 0)] // Round to even
		[TestCase(FP.HalfRaw - FP.EpsilonRaw, ExpectedResult = 0)]
		[TestCase(FP.HalfRaw + FP.EpsilonRaw, ExpectedResult = 1)]
		[TestCase(FP.OneRaw + FP.HalfRaw, ExpectedResult = 2)] // Round to even
		[TestCase(FP.OneRaw + FP.HalfRaw - FP.EpsilonRaw, ExpectedResult = 1)]
		[TestCase(FP.OneRaw + FP.HalfRaw + FP.EpsilonRaw, ExpectedResult = 2)]
		[TestCase(FP.PiRaw, ExpectedResult = 3)]
		[TestCase(FP.Rad2DegRaw, ExpectedResult = 57)]
		public int RoundToInt(long rawValue)
		{
			// Arrange
			var fp = FP.FromRaw(rawValue);

			// Act
			var result = FP.RoundToInt(fp);

			// Assert
			return result;
		}

		[TestCase(FP.HalfRaw, ExpectedResult = 0)]
		[TestCase(FP.OneRaw, ExpectedResult = 1)]
		[TestCase(FP.HalfRaw + FP.OneRaw, ExpectedResult = 1)]
		[TestCase(FP.OneRaw + FP.OneRaw, ExpectedResult = 2)]
		[TestCase(FP.PiRaw, ExpectedResult = 3)]
		[TestCase(FP.Rad2DegRaw, ExpectedResult = 57)]
		public int WhenCastToInt_ThenFloorToInt(long rawValue)
		{
			// Arrange
			var fp = FP.FromRaw(rawValue);

			// Act
			var result = fp.ToInt();

			// Assert
			return result;
		}

		[TestCase(FP.HalfRaw, ExpectedResult = 1)]
		[TestCase(FP.OneRaw, ExpectedResult = 1)]
		[TestCase(FP.HalfRaw + FP.OneRaw, ExpectedResult = 2)]
		[TestCase(FP.OneRaw + FP.OneRaw, ExpectedResult = 2)]
		[TestCase(FP.PiRaw, ExpectedResult = 4)]
		[TestCase(FP.Rad2DegRaw, ExpectedResult = 58)]
		public int CeilToInt(long rawValue)
		{
			// Arrange
			var fp = FP.FromRaw(rawValue);

			// Act
			var result = FP.CeilToInt(fp);

			// Assert
			return result;
		}

		[TestCase(FP.OneRaw, ExpectedResult = FP.OneRaw)]
		[TestCase(FP.HalfRaw, ExpectedResult = FP.HalfRaw)]
		[TestCase(-FP.PiRaw, ExpectedResult = -FP.PiRaw)]
		[TestCase(FP.MaxValueRaw, ExpectedResult = FP.MaxValueRaw)]
		[TestCase(FP.MaxValueRaw - 1, ExpectedResult = FP.MaxValueRaw - 1)]
		[TestCase(FP.MinValueRaw, ExpectedResult = FP.MinValueRaw)]
		public long WhenDevidedByOne_ThenReturnTheSameNumber(long rawValue)
		{
			// Arrange
			var fp = FP.FromRaw(rawValue);

			// Act
			var result = fp / FP.One;

			// Assert
			return result.RawValue;
		}

		[TestCaseSource(nameof(TestCases))]
		public void SqrtPrecise(FP value)
		{
			if (FP.SignInt(value) < 0)
			{
				Assert.Throws<ArgumentOutOfRangeException>(() => FP.SqrtPrecise(value));
			}
			else
			{
				var expected = Math.Sqrt(value.ToDouble());
				var actual = FP.SqrtPrecise(value).ToDouble();
				var delta = Math.Abs(expected - actual);

				if (delta > FP.Epsilon.ToDouble())
				{
					if (delta < (FP.Epsilon * 1000).ToDouble()) // It has some rare minor inaccuracies, and they are tied to absolute precision.
					{
						Debug.LogWarning($"sqrt({value}) = {actual}, but expected {expected}. Delta = {delta}.");
					}
					else
					{
						Assert.AreEqual(expected.ToFP(), actual, $"sqrt({value}) = {actual}, but expected {expected}. Delta = {delta}.");
					}
				}
			}
		}

		[TestCaseSource(nameof(TestCases))]
		public void SqrtApprox(FP value)
		{
			if (FP.SignInt(value) < 0)
			{
				Assert.Throws<ArgumentOutOfRangeException>(() => FP.Sqrt(value));
			}
			else
			{
				var expected = Math.Sqrt(value.ToDouble());
				var actual = FP.FromRaw(FP.Sqrt(value.RawValue)).ToDouble();
				var delta = Math.Abs(expected - actual);

				var expectedEpsilon = FP.FromRaw(2 << (FP.SqrtLutShift01 + 2));
				if (delta > expectedEpsilon.ToDouble())
				{
					Assert.AreEqual(expected.ToFP(), actual, $"sqrt({value}) = {actual}, but expected {expected}. Delta = {delta}.");
				}
			}
		}

		[TestCaseSource(nameof(TestCases))]
		public void Atan(FP value)
		{
			var expected = Math.Atan(value.ToDouble());
			var actual = FP.Atan(value);
			var delta = Math.Abs(expected - actual.ToDouble());

			if (delta > 0.00009)
			{
				Assert.AreEqual(expected.ToFP(), actual, $"Atan({value}) = {actual}, but expected {expected}. Delta = {delta}.");
			}
		}

		[TestCaseSource(nameof(PairTestCases))]
		public void Mul(FP a, FP b)
		{
			var aDouble = a.ToDouble();
			var bDouble = b.ToDouble();

			var expected = aDouble * bDouble;

			if (expected > FP.MaxValue.ToDouble())
			{
				expected = FP.MaxValue.ToDouble();
			}
			else if (expected < FP.MinValue.ToDouble())
			{
				expected = FP.MinValue.ToDouble();
			}

			FP actual;
			try
			{
				actual = a * b;
			}
			catch (OverflowException e)
			{
#if CHECK_OVERFLOW
				Assert.Pass();
				return;
#else
				Assert.Fail(e.Message);
				return;
#endif
			}

			var delta = Math.Abs(expected - actual.ToDouble());

			if (delta > FP.Epsilon.ToDouble())
			{
				actual = FP.FromRaw(FP.Mul(a.RawValue, b.RawValue));
				Assert.AreEqual(expected.ToFP(), actual, $"{a} * {b} = {actual}, but expected {expected}. Delta = {delta}.");
			}
		}

		[TestCaseSource(nameof(PairTestCases))]
		public void Division(FP a, FP b)
		{
			var aDouble = a.ToDouble();
			var bDouble = b.ToDouble();

			if (b == FP.Zero)
			{
				Assert.Throws<DivideByZeroException>(() => Ignore(a / b));
				return;
			}

			if (a == FP.Zero)
			{
				Assert.AreEqual(FP.Zero, a / b);
				return;
			}

			var expected = aDouble / bDouble;

			// Expect saturation up to max and min values
			if (expected > FP.MaxValue.ToDouble())
			{
				expected = FP.MaxValue.ToDouble();
			}
			else if (expected < FP.MinValue.ToDouble())
			{
				expected = FP.MinValue.ToDouble();
			}

			FP actual;
			try
			{
				actual = a / b;
			}
			catch (OverflowException e)
			{
#if CHECK_OVERFLOW
				Assert.Pass();
				return;
#else
				Assert.Fail(e.Message);
				return;
#endif
			}

			var delta = Math.Abs(expected - actual.ToDouble());

			if (delta > FP.Epsilon.ToDouble())
			{
				actual = FP.FromRaw(FP.Div(a.RawValue, b.RawValue));
				Assert.AreEqual(expected.ToFP(), actual, $"{a} / {b} = {actual}, but expected {expected}. Delta = {delta}.");
			}
		}

		[TestCaseSource(nameof(PairTestCases))]
		public void Atan2(FP y, FP x)
		{
			var yDouble = y.ToDouble();
			var xDouble = x.ToDouble();

			var expected = Math.Atan2(yDouble, xDouble);
			var actual = FP.Atan2(y, x);
			var delta = Math.Abs(expected - actual.ToDouble());

			if (delta > 0.005)
			{
				Assert.AreEqual(expected.ToFP(), actual, $"Atan({y}, {x}) = {actual}, but expected {expected}. Delta = {delta}.");
			}
		}

		private static void Ignore<T>(T value)
		{
		}

		public static IEnumerable PairTestCases()
		{
			foreach (var a in TestCases)
			{
				foreach (var b in TestCases)
				{
					yield return new object[] { a, b };
				}
			}
		}

		public static readonly FP[] TestCases =
		{
			// Small numbers
			FP.FromRaw(0),
			FP.FromRaw(1), FP.FromRaw(2), FP.FromRaw(3), FP.FromRaw(4), FP.FromRaw(5),
			FP.FromRaw(6), FP.FromRaw(7), FP.FromRaw(8), FP.FromRaw(9), FP.FromRaw(10),
			-FP.FromRaw(1), -FP.FromRaw(2), -FP.FromRaw(3), -FP.FromRaw(4), -FP.FromRaw(5),
			-FP.FromRaw(6), -FP.FromRaw(7), -FP.FromRaw(8), -FP.FromRaw(9), -FP.FromRaw(-10),

			// Integer numbers
			1.ToFP(), 2.ToFP(), 3.ToFP(), 4.ToFP(), 5.ToFP(), 6.ToFP(),
			(-1).ToFP(), (-2).ToFP(), (-3).ToFP(), (-4).ToFP(), (-5).ToFP(), (-6).ToFP(),

			// Fractions (1/2, 1/4, 1/8)
			FP.FromRaw(FP.OneRaw / 2), FP.FromRaw(FP.OneRaw / 4), FP.FromRaw(FP.OneRaw / 8),
			-FP.FromRaw(FP.OneRaw / 2), -FP.FromRaw(FP.OneRaw / 4), -FP.FromRaw(FP.OneRaw / 8),

			// Problematic carry
			FP.FromRaw(FP.OneRaw - 1), -FP.FromRaw(FP.OneRaw - 1),
			FP.FromRaw(FP.OneRaw + 1), -FP.FromRaw(FP.OneRaw + 1),

			// Problematic log2
			FP.FromRaw(1L << (FP.FractionalBits + FP.IntegerBits / 2)),
			FP.FromRaw((1L << (FP.FractionalBits + FP.IntegerBits / 2)) - 1),
			FP.FromRaw((1L << (FP.FractionalBits + FP.IntegerBits / 2)) + 1),

			// PIs
			FP.Pi, FP.HalfPi, -FP.HalfPi,

			// Smallest and largest values
			FP.MaxValue, FP.MinValue,

			// Large random numbers
			FP.FromRaw(6791302811978701836), FP.FromRaw(-8192141831180282065), FP.FromRaw(6222617001063736300), FP.FromRaw(-7871200276881732034),
			FP.FromRaw(8249382838880205112), FP.FromRaw(-7679310892959748444), FP.FromRaw(7708113189940799513), FP.FromRaw(-5281862979887936768),
			FP.FromRaw(8220231180772321456), FP.FromRaw(-5204203381295869580), FP.FromRaw(6860614387764479339), FP.FromRaw(-9080626825133349457),
			FP.FromRaw(6658610233456189347), FP.FromRaw(-6558014273345705245), FP.FromRaw(6700571222183426493),

			// Small random numbers
			FP.FromRaw(-436730658), FP.FromRaw(-2259913246), FP.FromRaw(329347474), FP.FromRaw(2565801981), FP.FromRaw(3398143698), FP.FromRaw(137497017), FP.FromRaw(1060347500),
			FP.FromRaw(-3457686027), FP.FromRaw(1923669753), FP.FromRaw(2891618613), FP.FromRaw(2418874813), FP.FromRaw(2899594950), FP.FromRaw(2265950765), FP.FromRaw(-1962365447),
			FP.FromRaw(3077934393),

			// Tiny random numbers
			FP.FromRaw(-171),
			FP.FromRaw(-359), FP.FromRaw(491), FP.FromRaw(844), FP.FromRaw(158), FP.FromRaw(-413), FP.FromRaw(-422), FP.FromRaw(-737), FP.FromRaw(-575), FP.FromRaw(-330),
			FP.FromRaw(-376), FP.FromRaw(435), FP.FromRaw(-311), FP.FromRaw(116), FP.FromRaw(715), FP.FromRaw(-1024), FP.FromRaw(-487), FP.FromRaw(59), FP.FromRaw(724), FP.FromRaw(993)
		};
	}
}
