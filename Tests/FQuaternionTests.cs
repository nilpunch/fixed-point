using NUnit.Framework;

namespace Mathematics.Fixed
{
	[TestFixture]
	public class FQuaternionTests
	{
		[Test]
		public static void WhenDot_AndRotationsAreTheSame_ThenEqualOne()
		{
			// Arrange
			var rotation180 = new FQuaternion(FP.One, FP.Zero, FP.Zero, FP.Zero);

			// Act
			FP dot = FQuaternion.Dot(rotation180, rotation180);

			// Assert
			Assert.IsTrue(FMath.ApproximatelyEqual(dot, FP.One));
		}

		[Test]
		public static void WhenDot_AndRotationsAreOpposite_ThenEqualNegativeOne()
		{
			// Arrange
			var rotation180 = new FQuaternion(FP.One, FP.Zero, FP.Zero, FP.Zero);
			var rotationMinus180 = new FQuaternion(-FP.One, FP.Zero, FP.Zero, FP.Zero);

			// Act
			FP negativeDot = FQuaternion.Dot(rotation180, rotationMinus180);

			// Assert
			Assert.IsTrue(FMath.ApproximatelyEqual(negativeDot, FP.MinusOne));
		}

		[Test]
		public static void When180RotationAppliedToVector_ThenFlipTheVector()
		{
			// Arrange
			FVector3 point = FVector3.Up;
			FQuaternion rotation180 = new FQuaternion(FP.One, FP.Zero, FP.Zero, FP.Zero);

			// Act
			FVector3 transformed = rotation180 * point;

			// Assert
			Assert.IsTrue(transformed == FVector3.Down);
		}

		[Test]
		public static void WhenScaledRotationAppliedToVector_ThenScaleTheVector()
		{
			// Arrange
			FVector3 point = FVector3.Up;
			FQuaternion scaling9 = new FQuaternion(FP.Zero, FP.Zero, FP.Zero, 3.ToFP());

			// Act
			FVector3 transformed = FQuaternion.Sandwich(scaling9, point);

			// Assert
			Assert.IsTrue(transformed == FVector3.Up * 9.ToFP());
		}
	}
}
