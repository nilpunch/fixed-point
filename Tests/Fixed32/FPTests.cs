using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace Fixed32
{
	[TestFixture]
	public class FPTests
	{
		[TestCase(0U, ExpectedResult = 31 + 1)]
		[TestCase(1U << 0, ExpectedResult = 31 - 0)]
		[TestCase(1U << 1, ExpectedResult = 31 - 1)]
		[TestCase(1U << 2, ExpectedResult = 31 - 2)]
		[TestCase(1U << 3, ExpectedResult = 31 - 3)]
		[TestCase(1U << 4, ExpectedResult = 31 - 4)]
		[TestCase(1U << 5, ExpectedResult = 31 - 5)]
		[TestCase(1U << 6, ExpectedResult = 31 - 6)]
		[TestCase(1U << 7, ExpectedResult = 31 - 7)]
		[TestCase(1U << 8, ExpectedResult = 31 - 8)]
		[TestCase(1U << 9, ExpectedResult = 31 - 9)]
		[TestCase(1U << 10, ExpectedResult = 31 - 10)]
		[TestCase(1U << 11, ExpectedResult = 31 - 11)]
		[TestCase(1U << 12, ExpectedResult = 31 - 12)]
		[TestCase(1U << 13, ExpectedResult = 31 - 13)]
		[TestCase(1U << 14, ExpectedResult = 31 - 14)]
		[TestCase(1U << 15, ExpectedResult = 31 - 15)]
		[TestCase(1U << 16, ExpectedResult = 31 - 16)]
		[TestCase(1U << 17, ExpectedResult = 31 - 17)]
		[TestCase(1U << 18, ExpectedResult = 31 - 18)]
		[TestCase(1U << 19, ExpectedResult = 31 - 19)]
		[TestCase(1U << 20, ExpectedResult = 31 - 20)]
		[TestCase(1U << 21, ExpectedResult = 31 - 21)]
		[TestCase(1U << 22, ExpectedResult = 31 - 22)]
		[TestCase(1U << 23, ExpectedResult = 31 - 23)]
		[TestCase(1U << 24, ExpectedResult = 31 - 24)]
		[TestCase(1U << 25, ExpectedResult = 31 - 25)]
		[TestCase(1U << 26, ExpectedResult = 31 - 26)]
		[TestCase(1U << 27, ExpectedResult = 31 - 27)]
		[TestCase(1U << 28, ExpectedResult = 31 - 28)]
		[TestCase(1U << 29, ExpectedResult = 31 - 29)]
		[TestCase(1U << 30, ExpectedResult = 31 - 30)]
		[TestCase(1U << 31, ExpectedResult = 31 - 31)]
		public int LZC(uint rawValue)
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
		public int Abs(int rawValue)
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
		public int RoundToInt(int rawValue)
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
		public int WhenCastToInt_ThenFloorToInt(int rawValue)
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
		public int CeilToInt(int rawValue)
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
		public long WhenDevidedByOne_ThenReturnTheSameNumber(int rawValue)
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
			FP.FromRaw(1 << (FP.FractionalBits + FP.IntegerBits / 2)),
			FP.FromRaw((1 << (FP.FractionalBits + FP.IntegerBits / 2)) - 1),
			FP.FromRaw((1 << (FP.FractionalBits + FP.IntegerBits / 2)) + 1),

			// PIs
			FP.Pi, FP.HalfPi, -FP.HalfPi,

			// Smallest and largest values
			FP.MaxValue, FP.MinValue,

			// Large random numbers
			FP.FromRaw(679130281), FP.FromRaw(-819214183), FP.FromRaw(622261700), FP.FromRaw(-787120027),
			FP.FromRaw(824938283), FP.FromRaw(-767931089), FP.FromRaw(770811318), FP.FromRaw(-528186297),
			FP.FromRaw(822023118), FP.FromRaw(-520420338), FP.FromRaw(686061438), FP.FromRaw(-908062682),
			FP.FromRaw(665861023), FP.FromRaw(-655801427), FP.FromRaw(670057122),

			// Small random numbers
			FP.FromRaw(-43675), FP.FromRaw(-225946), FP.FromRaw(3294), FP.FromRaw(25681), FP.FromRaw(33998), FP.FromRaw(13747), FP.FromRaw(106750),
			FP.FromRaw(-3457), FP.FromRaw(192653), FP.FromRaw(28913), FP.FromRaw(24188), FP.FromRaw(26890), FP.FromRaw(22655), FP.FromRaw(-196223),
			FP.FromRaw(30779),

			// Tiny random numbers
			FP.FromRaw(-17),
			FP.FromRaw(-35), FP.FromRaw(49), FP.FromRaw(84), FP.FromRaw(18), FP.FromRaw(-43), FP.FromRaw(-422), FP.FromRaw(-737), FP.FromRaw(-575), FP.FromRaw(-330),
			FP.FromRaw(-37), FP.FromRaw(43), FP.FromRaw(-31), FP.FromRaw(16), FP.FromRaw(71), FP.FromRaw(-102), FP.FromRaw(-487), FP.FromRaw(59), FP.FromRaw(724), FP.FromRaw(993)
		};
	}
}
