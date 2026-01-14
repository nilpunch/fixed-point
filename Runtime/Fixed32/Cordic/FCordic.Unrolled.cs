using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

// ReSharper disable all ShiftExpressionResultEqualsZero
namespace Fixed32
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
		public static void CordicCircular16(ref int xRef, ref int yRef, ref int zRef)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			if (z >= 0)
			{
				var xNext = x - (y >> 0);
				y = y + (x >> 0);
				x = xNext;
				z -= 843314856 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext = x + (y >> 0);
				y = y - (x >> 0);
				x = xNext;
				z += 843314856 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext1 = x - (y >> 1);
				y = y + (x >> 1);
				x = xNext1;
				z -= 497837829 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext2 = x + (y >> 1);
				y = y - (x >> 1);
				x = xNext2;
				z += 497837829 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext3 = x - (y >> 2);
				y = y + (x >> 2);
				x = xNext3;
				z -= 263043836 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext4 = x + (y >> 2);
				y = y - (x >> 2);
				x = xNext4;
				z += 263043836 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext5 = x - (y >> 3);
				y = y + (x >> 3);
				x = xNext5;
				z -= 133525158 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext6 = x + (y >> 3);
				y = y - (x >> 3);
				x = xNext6;
				z += 133525158 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext7 = x - (y >> 4);
				y = y + (x >> 4);
				x = xNext7;
				z -= 67021686 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext8 = x + (y >> 4);
				y = y - (x >> 4);
				x = xNext8;
				z += 67021686 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext9 = x - (y >> 5);
				y = y + (x >> 5);
				x = xNext9;
				z -= 33543515 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext10 = x + (y >> 5);
				y = y - (x >> 5);
				x = xNext10;
				z += 33543515 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext11 = x - (y >> 6);
				y = y + (x >> 6);
				x = xNext11;
				z -= 16775850 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext12 = x + (y >> 6);
				y = y - (x >> 6);
				x = xNext12;
				z += 16775850 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext13 = x - (y >> 7);
				y = y + (x >> 7);
				x = xNext13;
				z -= 8388437 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext14 = x + (y >> 7);
				y = y - (x >> 7);
				x = xNext14;
				z += 8388437 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext15 = x - (y >> 8);
				y = y + (x >> 8);
				x = xNext15;
				z -= 4194282 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext16 = x + (y >> 8);
				y = y - (x >> 8);
				x = xNext16;
				z += 4194282 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext17 = x - (y >> 9);
				y = y + (x >> 9);
				x = xNext17;
				z -= 2097149 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext18 = x + (y >> 9);
				y = y - (x >> 9);
				x = xNext18;
				z += 2097149 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext19 = x - (y >> 10);
				y = y + (x >> 10);
				x = xNext19;
				z -= 1048575 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext20 = x + (y >> 10);
				y = y - (x >> 10);
				x = xNext20;
				z += 1048575 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext21 = x - (y >> 11);
				y = y + (x >> 11);
				x = xNext21;
				z -= 524287 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext22 = x + (y >> 11);
				y = y - (x >> 11);
				x = xNext22;
				z += 524287 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext23 = x - (y >> 12);
				y = y + (x >> 12);
				x = xNext23;
				z -= 262143 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext24 = x + (y >> 12);
				y = y - (x >> 12);
				x = xNext24;
				z += 262143 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext25 = x - (y >> 13);
				y = y + (x >> 13);
				x = xNext25;
				z -= 131071 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext26 = x + (y >> 13);
				y = y - (x >> 13);
				x = xNext26;
				z += 131071 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext27 = x - (y >> 14);
				y = y + (x >> 14);
				x = xNext27;
				z -= 65535 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext28 = x + (y >> 14);
				y = y - (x >> 14);
				x = xNext28;
				z += 65535 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext29 = x - (y >> 15);
				y = y + (x >> 15);
				x = xNext29;
				z -= 32767 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext30 = x + (y >> 15);
				y = y - (x >> 15);
				x = xNext30;
				z += 32767 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext31 = x - (y >> 16);
				y = y + (x >> 16);
				x = xNext31;
				z -= 16383 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext32 = x + (y >> 16);
				y = y - (x >> 16);
				x = xNext32;
				z += 16383 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext33 = x - (y >> 17);
				y = y + (x >> 17);
				x = xNext33;
				z -= 8191 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext34 = x + (y >> 17);
				y = y - (x >> 17);
				x = xNext34;
				z += 8191 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext35 = x - (y >> 18);
				y = y + (x >> 18);
				x = xNext35;
				z -= 4095 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext36 = x + (y >> 18);
				y = y - (x >> 18);
				x = xNext36;
				z += 4095 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext37 = x - (y >> 19);
				y = y + (x >> 19);
				x = xNext37;
				z -= 2047 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext38 = x + (y >> 19);
				y = y - (x >> 19);
				x = xNext38;
				z += 2047 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext39 = x - (y >> 20);
				y = y + (x >> 20);
				x = xNext39;
				z -= 1023 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext40 = x + (y >> 20);
				y = y - (x >> 20);
				x = xNext40;
				z += 1023 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext41 = x - (y >> 21);
				y = y + (x >> 21);
				x = xNext41;
				z -= 511 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext42 = x + (y >> 21);
				y = y - (x >> 21);
				x = xNext42;
				z += 511 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext43 = x - (y >> 22);
				y = y + (x >> 22);
				x = xNext43;
				z -= 255 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext44 = x + (y >> 22);
				y = y - (x >> 22);
				x = xNext44;
				z += 255 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext45 = x - (y >> 23);
				y = y + (x >> 23);
				x = xNext45;
				z -= 127 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext46 = x + (y >> 23);
				y = y - (x >> 23);
				x = xNext46;
				z += 127 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext47 = x - (y >> 24);
				y = y + (x >> 24);
				x = xNext47;
				z -= 63 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext48 = x + (y >> 24);
				y = y - (x >> 24);
				x = xNext48;
				z += 63 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext49 = x - (y >> 25);
				y = y + (x >> 25);
				x = xNext49;
				z -= 31 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext50 = x + (y >> 25);
				y = y - (x >> 25);
				x = xNext50;
				z += 31 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext51 = x - (y >> 26);
				y = y + (x >> 26);
				x = xNext51;
				z -= 15 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext52 = x + (y >> 26);
				y = y - (x >> 26);
				x = xNext52;
				z += 15 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext53 = x - (y >> 27);
				y = y + (x >> 27);
				x = xNext53;
				z -= 8 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext54 = x + (y >> 27);
				y = y - (x >> 27);
				x = xNext54;
				z += 8 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext55 = x - (y >> 28);
				y = y + (x >> 28);
				x = xNext55;
				z -= 4 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext56 = x + (y >> 28);
				y = y - (x >> 28);
				x = xNext56;
				z += 4 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext57 = x - (y >> 29);
				y = y + (x >> 29);
				x = xNext57;
				z -= 2 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext58 = x + (y >> 29);
				y = y - (x >> 29);
				x = xNext58;
				z += 2 >> (30 - FP.FractionalBits);
			}
			if (z >= 0)
			{
				var xNext59 = x - (y >> 30);
				y = y + (x >> 30);
				x = xNext59;
				z -= 1 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext60 = x + (y >> 30);
				y = y - (x >> 30);
				x = xNext60;
				z += 1 >> (30 - FP.FractionalBits);
			}

			xRef = x;
			yRef = y;
			zRef = z;
		}

		/// <summary>
		/// z = [0, Pi/2]
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CordicVectoring16(ref int xRef, ref int yRef, ref int zRef, int target)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			if (y < target)
			{
				var xNext = x - (y >> 0);
				y = y + (x >> 0);
				x = xNext;
				z -= 843314856 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext = x + (y >> 0);
				y = y - (x >> 0);
				x = xNext;
				z += 843314856 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext1 = x - (y >> 1);
				y = y + (x >> 1);
				x = xNext1;
				z -= 497837829 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext2 = x + (y >> 1);
				y = y - (x >> 1);
				x = xNext2;
				z += 497837829 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext3 = x - (y >> 2);
				y = y + (x >> 2);
				x = xNext3;
				z -= 263043836 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext4 = x + (y >> 2);
				y = y - (x >> 2);
				x = xNext4;
				z += 263043836 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext5 = x - (y >> 3);
				y = y + (x >> 3);
				x = xNext5;
				z -= 133525158 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext6 = x + (y >> 3);
				y = y - (x >> 3);
				x = xNext6;
				z += 133525158 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext7 = x - (y >> 4);
				y = y + (x >> 4);
				x = xNext7;
				z -= 67021686 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext8 = x + (y >> 4);
				y = y - (x >> 4);
				x = xNext8;
				z += 67021686 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext9 = x - (y >> 5);
				y = y + (x >> 5);
				x = xNext9;
				z -= 33543515 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext10 = x + (y >> 5);
				y = y - (x >> 5);
				x = xNext10;
				z += 33543515 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext11 = x - (y >> 6);
				y = y + (x >> 6);
				x = xNext11;
				z -= 16775850 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext12 = x + (y >> 6);
				y = y - (x >> 6);
				x = xNext12;
				z += 16775850 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext13 = x - (y >> 7);
				y = y + (x >> 7);
				x = xNext13;
				z -= 8388437 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext14 = x + (y >> 7);
				y = y - (x >> 7);
				x = xNext14;
				z += 8388437 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext15 = x - (y >> 8);
				y = y + (x >> 8);
				x = xNext15;
				z -= 4194282 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext16 = x + (y >> 8);
				y = y - (x >> 8);
				x = xNext16;
				z += 4194282 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext17 = x - (y >> 9);
				y = y + (x >> 9);
				x = xNext17;
				z -= 2097149 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext18 = x + (y >> 9);
				y = y - (x >> 9);
				x = xNext18;
				z += 2097149 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext19 = x - (y >> 10);
				y = y + (x >> 10);
				x = xNext19;
				z -= 1048575 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext20 = x + (y >> 10);
				y = y - (x >> 10);
				x = xNext20;
				z += 1048575 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext21 = x - (y >> 11);
				y = y + (x >> 11);
				x = xNext21;
				z -= 524287 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext22 = x + (y >> 11);
				y = y - (x >> 11);
				x = xNext22;
				z += 524287 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext23 = x - (y >> 12);
				y = y + (x >> 12);
				x = xNext23;
				z -= 262143 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext24 = x + (y >> 12);
				y = y - (x >> 12);
				x = xNext24;
				z += 262143 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext25 = x - (y >> 13);
				y = y + (x >> 13);
				x = xNext25;
				z -= 131071 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext26 = x + (y >> 13);
				y = y - (x >> 13);
				x = xNext26;
				z += 131071 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext27 = x - (y >> 14);
				y = y + (x >> 14);
				x = xNext27;
				z -= 65535 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext28 = x + (y >> 14);
				y = y - (x >> 14);
				x = xNext28;
				z += 65535 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext29 = x - (y >> 15);
				y = y + (x >> 15);
				x = xNext29;
				z -= 32767 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext30 = x + (y >> 15);
				y = y - (x >> 15);
				x = xNext30;
				z += 32767 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext31 = x - (y >> 16);
				y = y + (x >> 16);
				x = xNext31;
				z -= 16383 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext32 = x + (y >> 16);
				y = y - (x >> 16);
				x = xNext32;
				z += 16383 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext33 = x - (y >> 17);
				y = y + (x >> 17);
				x = xNext33;
				z -= 8191 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext34 = x + (y >> 17);
				y = y - (x >> 17);
				x = xNext34;
				z += 8191 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext35 = x - (y >> 18);
				y = y + (x >> 18);
				x = xNext35;
				z -= 4095 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext36 = x + (y >> 18);
				y = y - (x >> 18);
				x = xNext36;
				z += 4095 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext37 = x - (y >> 19);
				y = y + (x >> 19);
				x = xNext37;
				z -= 2047 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext38 = x + (y >> 19);
				y = y - (x >> 19);
				x = xNext38;
				z += 2047 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext39 = x - (y >> 20);
				y = y + (x >> 20);
				x = xNext39;
				z -= 1023 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext40 = x + (y >> 20);
				y = y - (x >> 20);
				x = xNext40;
				z += 1023 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext41 = x - (y >> 21);
				y = y + (x >> 21);
				x = xNext41;
				z -= 511 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext42 = x + (y >> 21);
				y = y - (x >> 21);
				x = xNext42;
				z += 511 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext43 = x - (y >> 22);
				y = y + (x >> 22);
				x = xNext43;
				z -= 255 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext44 = x + (y >> 22);
				y = y - (x >> 22);
				x = xNext44;
				z += 255 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext45 = x - (y >> 23);
				y = y + (x >> 23);
				x = xNext45;
				z -= 127 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext46 = x + (y >> 23);
				y = y - (x >> 23);
				x = xNext46;
				z += 127 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext47 = x - (y >> 24);
				y = y + (x >> 24);
				x = xNext47;
				z -= 63 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext48 = x + (y >> 24);
				y = y - (x >> 24);
				x = xNext48;
				z += 63 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext49 = x - (y >> 25);
				y = y + (x >> 25);
				x = xNext49;
				z -= 31 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext50 = x + (y >> 25);
				y = y - (x >> 25);
				x = xNext50;
				z += 31 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext51 = x - (y >> 26);
				y = y + (x >> 26);
				x = xNext51;
				z -= 15 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext52 = x + (y >> 26);
				y = y - (x >> 26);
				x = xNext52;
				z += 15 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext53 = x - (y >> 27);
				y = y + (x >> 27);
				x = xNext53;
				z -= 8 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext54 = x + (y >> 27);
				y = y - (x >> 27);
				x = xNext54;
				z += 8 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext55 = x - (y >> 28);
				y = y + (x >> 28);
				x = xNext55;
				z -= 4 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext56 = x + (y >> 28);
				y = y - (x >> 28);
				x = xNext56;
				z += 4 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext57 = x - (y >> 29);
				y = y + (x >> 29);
				x = xNext57;
				z -= 2 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext58 = x + (y >> 29);
				y = y - (x >> 29);
				x = xNext58;
				z += 2 >> (30 - FP.FractionalBits);
			}
			if (y < target)
			{
				var xNext59 = x - (y >> 30);
				y = y + (x >> 30);
				x = xNext59;
				z -= 1 >> (30 - FP.FractionalBits);
			}
			else
			{
				var xNext60 = x + (y >> 30);
				y = y - (x >> 30);
				x = xNext60;
				z += 1 >> (30 - FP.FractionalBits);
			}

			xRef = x;
			yRef = y;
			zRef = z;
		}

		private static void CordicCircularUnrollTemplate(ref int xRef, ref int yRef, ref int zRef, int target)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			IterationStep(0, 843314856 >> (30 - FP.FractionalBits));
			IterationStep(1, 497837829 >> (30 - FP.FractionalBits));
			IterationStep(2, 263043836 >> (30 - FP.FractionalBits));
			IterationStep(3, 133525158 >> (30 - FP.FractionalBits));
			IterationStep(4, 67021686 >> (30 - FP.FractionalBits));
			IterationStep(5, 33543515 >> (30 - FP.FractionalBits));
			IterationStep(6, 16775850 >> (30 - FP.FractionalBits));
			IterationStep(7, 8388437 >> (30 - FP.FractionalBits));
			IterationStep(8, 4194282 >> (30 - FP.FractionalBits));
			IterationStep(9, 2097149 >> (30 - FP.FractionalBits));
			IterationStep(10, 1048575 >> (30 - FP.FractionalBits));
			IterationStep(11, 524287 >> (30 - FP.FractionalBits));
			IterationStep(12, 262143 >> (30 - FP.FractionalBits));
			IterationStep(13, 131071 >> (30 - FP.FractionalBits));
			IterationStep(14, 65535 >> (30 - FP.FractionalBits));
			IterationStep(15, 32767 >> (30 - FP.FractionalBits));
			IterationStep(16, 16383 >> (30 - FP.FractionalBits));
			IterationStep(17, 8191 >> (30 - FP.FractionalBits));
			IterationStep(18, 4095 >> (30 - FP.FractionalBits));
			IterationStep(19, 2047 >> (30 - FP.FractionalBits));
			IterationStep(20, 1023 >> (30 - FP.FractionalBits));
			IterationStep(21, 511 >> (30 - FP.FractionalBits));
			IterationStep(22, 255 >> (30 - FP.FractionalBits));
			IterationStep(23, 127 >> (30 - FP.FractionalBits));
			IterationStep(24, 63 >> (30 - FP.FractionalBits));
			IterationStep(25, 31 >> (30 - FP.FractionalBits));
			IterationStep(26, 15 >> (30 - FP.FractionalBits));
			IterationStep(27, 8 >> (30 - FP.FractionalBits));
			IterationStep(28, 4 >> (30 - FP.FractionalBits));
			IterationStep(29, 2 >> (30 - FP.FractionalBits));
			IterationStep(30, 1 >> (30 - FP.FractionalBits));

			xRef = x;
			yRef = y;
			zRef = z;

			void IterationStep(int i, int atan)
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

		private static void CordicVectoringUnrollTemplate(ref int xRef, ref int yRef, ref int zRef, int target)
		{
			var x = xRef;
			var y = yRef;
			var z = zRef;

			IterationStep(0, 843314856 >> (30 - FP.FractionalBits));
			IterationStep(1, 497837829 >> (30 - FP.FractionalBits));
			IterationStep(2, 263043836 >> (30 - FP.FractionalBits));
			IterationStep(3, 133525158 >> (30 - FP.FractionalBits));
			IterationStep(4, 67021686 >> (30 - FP.FractionalBits));
			IterationStep(5, 33543515 >> (30 - FP.FractionalBits));
			IterationStep(6, 16775850 >> (30 - FP.FractionalBits));
			IterationStep(7, 8388437 >> (30 - FP.FractionalBits));
			IterationStep(8, 4194282 >> (30 - FP.FractionalBits));
			IterationStep(9, 2097149 >> (30 - FP.FractionalBits));
			IterationStep(10, 1048575 >> (30 - FP.FractionalBits));
			IterationStep(11, 524287 >> (30 - FP.FractionalBits));
			IterationStep(12, 262143 >> (30 - FP.FractionalBits));
			IterationStep(13, 131071 >> (30 - FP.FractionalBits));
			IterationStep(14, 65535 >> (30 - FP.FractionalBits));
			IterationStep(15, 32767 >> (30 - FP.FractionalBits));
			IterationStep(16, 16383 >> (30 - FP.FractionalBits));
			IterationStep(17, 8191 >> (30 - FP.FractionalBits));
			IterationStep(18, 4095 >> (30 - FP.FractionalBits));
			IterationStep(19, 2047 >> (30 - FP.FractionalBits));
			IterationStep(20, 1023 >> (30 - FP.FractionalBits));
			IterationStep(21, 511 >> (30 - FP.FractionalBits));
			IterationStep(22, 255 >> (30 - FP.FractionalBits));
			IterationStep(23, 127 >> (30 - FP.FractionalBits));
			IterationStep(24, 63 >> (30 - FP.FractionalBits));
			IterationStep(25, 31 >> (30 - FP.FractionalBits));
			IterationStep(26, 15 >> (30 - FP.FractionalBits));
			IterationStep(27, 8 >> (30 - FP.FractionalBits));
			IterationStep(28, 4 >> (30 - FP.FractionalBits));
			IterationStep(29, 2 >> (30 - FP.FractionalBits));
			IterationStep(30, 1 >> (30 - FP.FractionalBits));

			xRef = x;
			yRef = y;
			zRef = z;

			void IterationStep(int i, int atan)
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
