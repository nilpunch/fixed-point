// ReSharper disable all ShiftExpressionResultEqualsZero

using System;
using Unity.IL2CPP.CompilerServices;

namespace Fixed32
{
	public static partial class FCordic
	{
		/// <summary>
		/// Lookup for Math.Atan(2^-i) where i = [0, 30].
		/// </summary>
		public static readonly int[] RawAtans =
		{
			843314856 >> (30 - FP.FractionalBits),
			497837829 >> (30 - FP.FractionalBits),
			263043836 >> (30 - FP.FractionalBits),
			133525158 >> (30 - FP.FractionalBits),
			67021686 >> (30 - FP.FractionalBits),
			33543515 >> (30 - FP.FractionalBits),
			16775850 >> (30 - FP.FractionalBits),
			8388437 >> (30 - FP.FractionalBits),
			4194282 >> (30 - FP.FractionalBits),
			2097149 >> (30 - FP.FractionalBits),
			1048575 >> (30 - FP.FractionalBits),
			524287 >> (30 - FP.FractionalBits),
			262143 >> (30 - FP.FractionalBits),
			131071 >> (30 - FP.FractionalBits),
			65535 >> (30 - FP.FractionalBits),
			32767 >> (30 - FP.FractionalBits),
			16383 >> (30 - FP.FractionalBits),
			8191 >> (30 - FP.FractionalBits),
			4095 >> (30 - FP.FractionalBits),
			2047 >> (30 - FP.FractionalBits),
			1023 >> (30 - FP.FractionalBits),
			511 >> (30 - FP.FractionalBits),
			255 >> (30 - FP.FractionalBits),
			127 >> (30 - FP.FractionalBits),
			63 >> (30 - FP.FractionalBits),
			31 >> (30 - FP.FractionalBits),
			15 >> (30 - FP.FractionalBits),
			8 >> (30 - FP.FractionalBits),
			4 >> (30 - FP.FractionalBits),
			2 >> (30 - FP.FractionalBits),
			1 >> (30 - FP.FractionalBits),
		};

		/// <summary>
		/// Non-deterministic across boundaries. Must be baked in code.<br/>
		/// Lookup for Math.Atan(2^-i) where i = [0, 30].
		/// </summary>
		public static int[] GenerateRawAtansLookup()
		{
			var result = new int[31];

			for (var i = 0; i <= 30; i++)
			{
				var atan = Math.Atan(1.0 / Math.Pow(2, i));
				var raw = (int)(atan * (1 << 30));
				result[i] = raw;
			}

			return result;
		}

		/// <summary>
		/// Non-deterministic across boundaries. Must be baked in code.<br/>
		/// CORDIC cosine constant.
		/// </summary>
		public static int CalculateRawGain()
		{
			var cos = 1.0;

			for (var i = 0; i < 32; i++)
			{
				var result = Math.Atan(1.0 / Math.Pow(2, i));
				cos *= Math.Cos(result);
			}

			return (int)(cos * (1 << 30));
		}
	}
}
