using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

// ReSharper disable all ShiftExpressionResultEqualsZero
namespace Fixed64
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static partial class FCordic
	{
		/// <summary>
		/// z = [0, Pi/2]
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CordicCircular16(ref long xRef, ref long yRef, ref long zRef)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			if (z >= 0)
			{
				var xNext = x - (y >> 0);
				y = y + (x >> 0);
				x = xNext;
				z -= 7244019458077122560L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext = x + (y >> 0);
				y = y - (x >> 0);
				x = xNext;
				z += 7244019458077122560L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext1 = x - (y >> 1);
				y = y + (x >> 1);
				x = xNext1;
				z -= 4276394391812611584L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext2 = x + (y >> 1);
				y = y - (x >> 1);
				x = xNext2;
				z += 4276394391812611584L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext3 = x - (y >> 2);
				y = y + (x >> 2);
				x = xNext3;
				z -= 2259529351110384896L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext4 = x + (y >> 2);
				y = y - (x >> 2);
				x = xNext4;
				z += 2259529351110384896L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext5 = x - (y >> 3);
				y = y + (x >> 3);
				x = xNext5;
				z -= 1146972379345827584L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext6 = x + (y >> 3);
				y = y - (x >> 3);
				x = xNext6;
				z += 1146972379345827584L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext7 = x - (y >> 4);
				y = y + (x >> 4);
				x = xNext7;
				z -= 575711906690464384L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext8 = x + (y >> 4);
				y = y - (x >> 4);
				x = xNext8;
				z += 575711906690464384L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext9 = x - (y >> 5);
				y = y + (x >> 5);
				x = xNext9;
				z -= 288136606096737440L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext10 = x + (y >> 5);
				y = y - (x >> 5);
				x = xNext10;
				z += 288136606096737440L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext11 = x - (y >> 6);
				y = y + (x >> 6);
				x = xNext11;
				z -= 144103461669513648L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext12 = x + (y >> 6);
				y = y - (x >> 6);
				x = xNext12;
				z += 144103461669513648L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext13 = x - (y >> 7);
				y = y + (x >> 7);
				x = xNext13;
				z -= 72056128076108984L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext14 = x + (y >> 7);
				y = y - (x >> 7);
				x = xNext14;
				z += 72056128076108984L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext15 = x - (y >> 8);
				y = y + (x >> 8);
				x = xNext15;
				z -= 36028613768703708L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext16 = x + (y >> 8);
				y = y - (x >> 8);
				x = xNext16;
				z += 36028613768703708L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext17 = x - (y >> 9);
				y = y + (x >> 9);
				x = xNext17;
				z -= 18014375603042168L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext18 = x + (y >> 9);
				y = y - (x >> 9);
				x = xNext18;
				z += 18014375603042168L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext19 = x - (y >> 10);
				y = y + (x >> 10);
				x = xNext19;
				z -= 9007196391431100L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext20 = x + (y >> 10);
				y = y - (x >> 10);
				x = xNext20;
				z += 9007196391431100L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext21 = x - (y >> 11);
				y = y + (x >> 11);
				x = xNext21;
				z -= 4503599269456606L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext22 = x + (y >> 11);
				y = y - (x >> 11);
				x = xNext22;
				z += 4503599269456606L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext23 = x - (y >> 12);
				y = y + (x >> 12);
				x = xNext23;
				z -= 2251799768946007L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext24 = x + (y >> 12);
				y = y - (x >> 12);
				x = xNext24;
				z += 2251799768946007L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext25 = x - (y >> 13);
				y = y + (x >> 13);
				x = xNext25;
				z -= 1125899901250218L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext26 = x + (y >> 13);
				y = y - (x >> 13);
				x = xNext26;
				z += 1125899901250218L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext27 = x - (y >> 14);
				y = y + (x >> 14);
				x = xNext27;
				z -= 562949952722261L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext28 = x + (y >> 14);
				y = y - (x >> 14);
				x = xNext28;
				z += 562949952722261L >> (63 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext29 = x - (y >> 15);
				y = y + (x >> 15);
				x = xNext29;
				z -= 281474976623274L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext30 = x + (y >> 15);
				y = y - (x >> 15);
				x = xNext30;
				z += 281474976623274L >> (63 - FP.FractionalBits);
			}

			xRef = x;
			yRef = y;
			zRef = z;
		}

		/// <summary>
		/// z = [0, Pi/2]
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CordicVectoring16(ref long xRef, ref long yRef, ref long zRef, long target)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			if (y < target)
			{
				var xNext = x - (y >> 0);
				y = y + (x >> 0);
				x = xNext;
				z -= 7244019458077122560L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext = x + (y >> 0);
				y = y - (x >> 0);
				x = xNext;
				z += 7244019458077122560L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext1 = x - (y >> 1);
				y = y + (x >> 1);
				x = xNext1;
				z -= 4276394391812611584L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext2 = x + (y >> 1);
				y = y - (x >> 1);
				x = xNext2;
				z += 4276394391812611584L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext3 = x - (y >> 2);
				y = y + (x >> 2);
				x = xNext3;
				z -= 2259529351110384896L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext4 = x + (y >> 2);
				y = y - (x >> 2);
				x = xNext4;
				z += 2259529351110384896L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext5 = x - (y >> 3);
				y = y + (x >> 3);
				x = xNext5;
				z -= 1146972379345827584L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext6 = x + (y >> 3);
				y = y - (x >> 3);
				x = xNext6;
				z += 1146972379345827584L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext7 = x - (y >> 4);
				y = y + (x >> 4);
				x = xNext7;
				z -= 575711906690464384L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext8 = x + (y >> 4);
				y = y - (x >> 4);
				x = xNext8;
				z += 575711906690464384L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext9 = x - (y >> 5);
				y = y + (x >> 5);
				x = xNext9;
				z -= 288136606096737440L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext10 = x + (y >> 5);
				y = y - (x >> 5);
				x = xNext10;
				z += 288136606096737440L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext11 = x - (y >> 6);
				y = y + (x >> 6);
				x = xNext11;
				z -= 144103461669513648L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext12 = x + (y >> 6);
				y = y - (x >> 6);
				x = xNext12;
				z += 144103461669513648L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext13 = x - (y >> 7);
				y = y + (x >> 7);
				x = xNext13;
				z -= 72056128076108984L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext14 = x + (y >> 7);
				y = y - (x >> 7);
				x = xNext14;
				z += 72056128076108984L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext15 = x - (y >> 8);
				y = y + (x >> 8);
				x = xNext15;
				z -= 36028613768703708L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext16 = x + (y >> 8);
				y = y - (x >> 8);
				x = xNext16;
				z += 36028613768703708L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext17 = x - (y >> 9);
				y = y + (x >> 9);
				x = xNext17;
				z -= 18014375603042168L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext18 = x + (y >> 9);
				y = y - (x >> 9);
				x = xNext18;
				z += 18014375603042168L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext19 = x - (y >> 10);
				y = y + (x >> 10);
				x = xNext19;
				z -= 9007196391431100L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext20 = x + (y >> 10);
				y = y - (x >> 10);
				x = xNext20;
				z += 9007196391431100L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext21 = x - (y >> 11);
				y = y + (x >> 11);
				x = xNext21;
				z -= 4503599269456606L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext22 = x + (y >> 11);
				y = y - (x >> 11);
				x = xNext22;
				z += 4503599269456606L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext23 = x - (y >> 12);
				y = y + (x >> 12);
				x = xNext23;
				z -= 2251799768946007L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext24 = x + (y >> 12);
				y = y - (x >> 12);
				x = xNext24;
				z += 2251799768946007L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext25 = x - (y >> 13);
				y = y + (x >> 13);
				x = xNext25;
				z -= 1125899901250218L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext26 = x + (y >> 13);
				y = y - (x >> 13);
				x = xNext26;
				z += 1125899901250218L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext27 = x - (y >> 14);
				y = y + (x >> 14);
				x = xNext27;
				z -= 562949952722261L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext28 = x + (y >> 14);
				y = y - (x >> 14);
				x = xNext28;
				z += 562949952722261L >> (63 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext29 = x - (y >> 15);
				y = y + (x >> 15);
				x = xNext29;
				z -= 281474976623274L >> (63 - FP.FractionalBits);
			}
			else
			{
				var xNext30 = x + (y >> 15);
				y = y - (x >> 15);
				x = xNext30;
				z += 281474976623274L >> (63 - FP.FractionalBits);
			}

			xRef = x;
			yRef = y;
			zRef = z;
		}

		private static void CordicCircularUnrollTemplate(ref long xRef, ref long yRef, ref long zRef, long target)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			IterationStep(0, 7244019458077122560L >> (63 - FP.FractionalBits));
			IterationStep(1, 4276394391812611584L >> (63 - FP.FractionalBits));
			IterationStep(2, 2259529351110384896L >> (63 - FP.FractionalBits));
			IterationStep(3, 1146972379345827584L >> (63 - FP.FractionalBits));
			IterationStep(4, 575711906690464384L >> (63 - FP.FractionalBits));
			IterationStep(5, 288136606096737440L >> (63 - FP.FractionalBits));
			IterationStep(6, 144103461669513648L >> (63 - FP.FractionalBits));
			IterationStep(7, 72056128076108984L >> (63 - FP.FractionalBits));
			IterationStep(8, 36028613768703708L >> (63 - FP.FractionalBits));
			IterationStep(9, 18014375603042168L >> (63 - FP.FractionalBits));
			IterationStep(10, 9007196391431100L >> (63 - FP.FractionalBits));
			IterationStep(11, 4503599269456606L >> (63 - FP.FractionalBits));
			IterationStep(12, 2251799768946007L >> (63 - FP.FractionalBits));
			IterationStep(13, 1125899901250218L >> (63 - FP.FractionalBits));
			IterationStep(14, 562949952722261L >> (63 - FP.FractionalBits));
			IterationStep(15, 281474976623274L >> (63 - FP.FractionalBits));
			IterationStep(16, 140737488344405L >> (63 - FP.FractionalBits));
			IterationStep(17, 70368744176298L >> (63 - FP.FractionalBits));
			IterationStep(18, 35184372088661L >> (63 - FP.FractionalBits));
			IterationStep(19, 17592186044394L >> (63 - FP.FractionalBits));
			IterationStep(20, 8796093022205L >> (63 - FP.FractionalBits));
			IterationStep(21, 4398046511103L >> (63 - FP.FractionalBits));
			IterationStep(22, 2199023255551L >> (63 - FP.FractionalBits));
			IterationStep(23, 1099511627775L >> (63 - FP.FractionalBits));
			IterationStep(24, 549755813887L >> (63 - FP.FractionalBits));
			IterationStep(25, 274877906943L >> (63 - FP.FractionalBits));
			IterationStep(26, 137438953471L >> (63 - FP.FractionalBits));
			IterationStep(27, 68719476736L >> (63 - FP.FractionalBits));
			IterationStep(28, 34359738368L >> (63 - FP.FractionalBits));
			IterationStep(29, 17179869184L >> (63 - FP.FractionalBits));
			IterationStep(30, 8589934592L >> (63 - FP.FractionalBits));
			IterationStep(31, 4294967296L >> (63 - FP.FractionalBits));
			IterationStep(32, 2147483648L >> (63 - FP.FractionalBits));
			IterationStep(33, 1073741824L >> (63 - FP.FractionalBits));
			IterationStep(34, 536870912L >> (63 - FP.FractionalBits));
			IterationStep(35, 268435456L >> (63 - FP.FractionalBits));
			IterationStep(36, 134217728L >> (63 - FP.FractionalBits));
			IterationStep(37, 67108864L >> (63 - FP.FractionalBits));
			IterationStep(38, 33554432L >> (63 - FP.FractionalBits));
			IterationStep(39, 16777216L >> (63 - FP.FractionalBits));
			IterationStep(40, 8388608L >> (63 - FP.FractionalBits));
			IterationStep(41, 4194304L >> (63 - FP.FractionalBits));
			IterationStep(42, 2097152L >> (63 - FP.FractionalBits));
			IterationStep(43, 1048576L >> (63 - FP.FractionalBits));
			IterationStep(44, 524288L >> (63 - FP.FractionalBits));
			IterationStep(45, 262144L >> (63 - FP.FractionalBits));
			IterationStep(46, 131072L >> (63 - FP.FractionalBits));
			IterationStep(47, 65536L >> (63 - FP.FractionalBits));
			IterationStep(48, 32768L >> (63 - FP.FractionalBits));
			IterationStep(49, 16384L >> (63 - FP.FractionalBits));
			IterationStep(50, 8192L >> (63 - FP.FractionalBits));
			IterationStep(51, 4096L >> (63 - FP.FractionalBits));
			IterationStep(52, 2048L >> (63 - FP.FractionalBits));
			IterationStep(53, 1024L >> (63 - FP.FractionalBits));
			IterationStep(54, 512L >> (63 - FP.FractionalBits));
			IterationStep(55, 256L >> (63 - FP.FractionalBits));
			IterationStep(56, 128L >> (63 - FP.FractionalBits));
			IterationStep(57, 64L >> (63 - FP.FractionalBits));
			IterationStep(58, 32L >> (63 - FP.FractionalBits));
			IterationStep(59, 16L >> (63 - FP.FractionalBits));
			IterationStep(60, 8L >> (63 - FP.FractionalBits));
			IterationStep(61, 4L >> (63 - FP.FractionalBits));
			IterationStep(62, 2L >> (63 - FP.FractionalBits));
			IterationStep(63, 1L >> (63 - FP.FractionalBits));

			xRef = x;
			yRef = y;
			zRef = z;

			void IterationStep(int i, long atan)
			{
				if (z >= 0)
				{
					var xNext = x - (y >> i);
					y = y + (x >> i);
					x = xNext;
					z -= atan;
				}
				else
				{
					var xNext = x + (y >> i);
					y = y - (x >> i);
					x = xNext;
					z += atan;
				}
			}
		}

		private static void CordicVectoringUnrollTemplate(ref long xRef, ref long yRef, ref long zRef, long target)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			IterationStep(0, 7244019458077122560L >> (63 - FP.FractionalBits));
			IterationStep(1, 4276394391812611584L >> (63 - FP.FractionalBits));
			IterationStep(2, 2259529351110384896L >> (63 - FP.FractionalBits));
			IterationStep(3, 1146972379345827584L >> (63 - FP.FractionalBits));
			IterationStep(4, 575711906690464384L >> (63 - FP.FractionalBits));
			IterationStep(5, 288136606096737440L >> (63 - FP.FractionalBits));
			IterationStep(6, 144103461669513648L >> (63 - FP.FractionalBits));
			IterationStep(7, 72056128076108984L >> (63 - FP.FractionalBits));
			IterationStep(8, 36028613768703708L >> (63 - FP.FractionalBits));
			IterationStep(9, 18014375603042168L >> (63 - FP.FractionalBits));
			IterationStep(10, 9007196391431100L >> (63 - FP.FractionalBits));
			IterationStep(11, 4503599269456606L >> (63 - FP.FractionalBits));
			IterationStep(12, 2251799768946007L >> (63 - FP.FractionalBits));
			IterationStep(13, 1125899901250218L >> (63 - FP.FractionalBits));
			IterationStep(14, 562949952722261L >> (63 - FP.FractionalBits));
			IterationStep(15, 281474976623274L >> (63 - FP.FractionalBits));
			IterationStep(16, 140737488344405L >> (63 - FP.FractionalBits));
			IterationStep(17, 70368744176298L >> (63 - FP.FractionalBits));
			IterationStep(18, 35184372088661L >> (63 - FP.FractionalBits));
			IterationStep(19, 17592186044394L >> (63 - FP.FractionalBits));
			IterationStep(20, 8796093022205L >> (63 - FP.FractionalBits));
			IterationStep(21, 4398046511103L >> (63 - FP.FractionalBits));
			IterationStep(22, 2199023255551L >> (63 - FP.FractionalBits));
			IterationStep(23, 1099511627775L >> (63 - FP.FractionalBits));
			IterationStep(24, 549755813887L >> (63 - FP.FractionalBits));
			IterationStep(25, 274877906943L >> (63 - FP.FractionalBits));
			IterationStep(26, 137438953471L >> (63 - FP.FractionalBits));
			IterationStep(27, 68719476736L >> (63 - FP.FractionalBits));
			IterationStep(28, 34359738368L >> (63 - FP.FractionalBits));
			IterationStep(29, 17179869184L >> (63 - FP.FractionalBits));
			IterationStep(30, 8589934592L >> (63 - FP.FractionalBits));
			IterationStep(31, 4294967296L >> (63 - FP.FractionalBits));
			IterationStep(32, 2147483648L >> (63 - FP.FractionalBits));
			IterationStep(33, 1073741824L >> (63 - FP.FractionalBits));
			IterationStep(34, 536870912L >> (63 - FP.FractionalBits));
			IterationStep(35, 268435456L >> (63 - FP.FractionalBits));
			IterationStep(36, 134217728L >> (63 - FP.FractionalBits));
			IterationStep(37, 67108864L >> (63 - FP.FractionalBits));
			IterationStep(38, 33554432L >> (63 - FP.FractionalBits));
			IterationStep(39, 16777216L >> (63 - FP.FractionalBits));
			IterationStep(40, 8388608L >> (63 - FP.FractionalBits));
			IterationStep(41, 4194304L >> (63 - FP.FractionalBits));
			IterationStep(42, 2097152L >> (63 - FP.FractionalBits));
			IterationStep(43, 1048576L >> (63 - FP.FractionalBits));
			IterationStep(44, 524288L >> (63 - FP.FractionalBits));
			IterationStep(45, 262144L >> (63 - FP.FractionalBits));
			IterationStep(46, 131072L >> (63 - FP.FractionalBits));
			IterationStep(47, 65536L >> (63 - FP.FractionalBits));
			IterationStep(48, 32768L >> (63 - FP.FractionalBits));
			IterationStep(49, 16384L >> (63 - FP.FractionalBits));
			IterationStep(50, 8192L >> (63 - FP.FractionalBits));
			IterationStep(51, 4096L >> (63 - FP.FractionalBits));
			IterationStep(52, 2048L >> (63 - FP.FractionalBits));
			IterationStep(53, 1024L >> (63 - FP.FractionalBits));
			IterationStep(54, 512L >> (63 - FP.FractionalBits));
			IterationStep(55, 256L >> (63 - FP.FractionalBits));
			IterationStep(56, 128L >> (63 - FP.FractionalBits));
			IterationStep(57, 64L >> (63 - FP.FractionalBits));
			IterationStep(58, 32L >> (63 - FP.FractionalBits));
			IterationStep(59, 16L >> (63 - FP.FractionalBits));
			IterationStep(60, 8L >> (63 - FP.FractionalBits));
			IterationStep(61, 4L >> (63 - FP.FractionalBits));
			IterationStep(62, 2L >> (63 - FP.FractionalBits));
			IterationStep(63, 1L >> (63 - FP.FractionalBits));

			xRef = x;
			yRef = y;
			zRef = z;

			void IterationStep(int i, long atan)
			{
				if (y < target)
				{
					var xNext = x - (y >> i);
					y = y + (x >> i);
					x = xNext;
					z -= atan;
				}
				else
				{
					var xNext = x + (y >> i);
					y = y - (x >> i);
					x = xNext;
					z += atan;
				}
			}
		}
	}
}
