using NUnit.Framework;

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
		public int WhenRoundToInt_ThenRoundToInt(long rawValue)
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
		public int WhenCeilToInt_ThenCeilToInt(long rawValue)
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
		[TestCase(FP.MinValueRaw, ExpectedResult = FP.MinValueRaw)]
		public long WhenDevidedByOne_ShouldReturnTheSameNumber(long rawValue)
		{
			// Arrange
			var fp = FP.FromRaw(rawValue);

			// Act
			var result = fp / FP.One;

			// Assert
			return result.RawValue;
		}
	}
}
