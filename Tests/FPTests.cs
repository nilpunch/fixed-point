using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace Mathematics.Fixed
{
	[TestFixture]
	public class FPTests
	{
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
			var result = FMath.RoundToInt(fp);

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
			var result = FMath.CeilToInt(fp);

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
			if (FMath.SignInt(value) < 0)
			{
				Assert.Throws<ArgumentOutOfRangeException>(() => FMath.SqrtPrecise(value));
			}
			else
			{
				var expected = Math.Sqrt(value.ToDouble());
				var actual = FMath.SqrtPrecise(value).ToDouble();
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
			if (FMath.SignInt(value) < 0)
			{
				Assert.Throws<ArgumentOutOfRangeException>(() => FMath.Sqrt(value));
			}
			else
			{
				var expected = Math.Sqrt(value.ToDouble());
				var actual = FP.FromRaw(FMath.Sqrt(value.RawValue)).ToDouble();
				var delta = Math.Abs(expected - actual);

				var expectedEpsilon = FP.FromRaw(2 << (FMath.SqrtLutShift01 + 2));
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
			var actual = FMath.Atan(value);
			var delta = Math.Abs(expected - actual.ToDouble());

			if (delta > 0.00000001)
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

			var actual = a * b;
			var delta = Math.Abs(expected - actual.ToDouble());

			if (delta > FP.Epsilon.ToDouble())
			{
				if (delta < (FP.Epsilon * 2000).ToDouble()) // It has some rare minor inaccuracies, and they are tied to absolute precision.
				{
					Debug.LogWarning($"{a} * {b} = {actual}, but expected {expected}. Delta = {delta}.");
				}
				else
				{
					Assert.AreEqual(expected.ToFP(), actual, $"{a} * {b} = {actual}, but expected {expected}. Delta = {delta}.");
				}
			}
		}

		[TestCaseSource(nameof(PairTestCases))]
		public void Division(FP a, FP b)
		{
			var aDouble = a.ToDouble();
			var bDouble = b.ToDouble();

			if (a == FP.Zero)
			{
				var expected = FP.Zero;
				Assert.AreEqual(expected, a / b);
				return;
			}

			if (b == FP.Zero)
			{
				// Assert.Throws<DivideByZeroException>(() => Ignore(a / b));

				var expected = a < FP.Zero ? FP.MinValue : FP.MaxValue;
				Assert.AreEqual(expected, a / b);
				return;
			}

			{
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

				var actual = a / b;
				var delta = Math.Abs(expected - actual.ToDouble());

				if (delta > FP.Epsilon.ToDouble())
				{
					if (delta < (FP.Epsilon * 2000).ToDouble()) // It has some rare minor inaccuracies, and they are tied to absolute precision.
					{
						Debug.LogWarning($"{a} / {b} = {actual}, but expected {expected}. Delta = {delta}.");
					}
					else
					{
						Assert.AreEqual(expected.ToFP(), actual, $"{a} / {b} = {actual}, but expected {expected}. Delta = {delta}.");
					}
				}
			}
		}

		[TestCaseSource(nameof(PairTestCases))]
		public void Atan2(FP y, FP x)
		{
			var yDouble = y.ToDouble();
			var xDouble = x.ToDouble();

			var expected = Math.Atan2(yDouble, xDouble);
			var actual = FMath.Atan2(y, x);
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
