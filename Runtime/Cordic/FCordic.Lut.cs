// ReSharper disable all ShiftExpressionResultEqualsZero

using Unity.IL2CPP.CompilerServices;

namespace Mathematics.Fixed
{
	public static partial class FCordic
	{
		/// <summary>
		/// 64 iterations of Math.Atan(2^-i) where i = 0 .. 63.
		/// </summary>
		public static readonly long[] RawAtans =
		{
			7244019458077122560L >> (63 - FP.FractionalBits),
			4276394391812611584L >> (63 - FP.FractionalBits),
			2259529351110384896L >> (63 - FP.FractionalBits),
			1146972379345827584L >> (63 - FP.FractionalBits),
			575711906690464384L >> (63 - FP.FractionalBits),
			288136606096737440L >> (63 - FP.FractionalBits),
			144103461669513648L >> (63 - FP.FractionalBits),
			72056128076108984L >> (63 - FP.FractionalBits),
			36028613768703708L >> (63 - FP.FractionalBits),
			18014375603042168L >> (63 - FP.FractionalBits),
			9007196391431100L >> (63 - FP.FractionalBits),
			4503599269456606L >> (63 - FP.FractionalBits),
			2251799768946007L >> (63 - FP.FractionalBits),
			1125899901250218L >> (63 - FP.FractionalBits),
			562949952722261L >> (63 - FP.FractionalBits),
			281474976623274L >> (63 - FP.FractionalBits),
			140737488344405L >> (63 - FP.FractionalBits),
			70368744176298L >> (63 - FP.FractionalBits),
			35184372088661L >> (63 - FP.FractionalBits),
			17592186044394L >> (63 - FP.FractionalBits),
			8796093022205L >> (63 - FP.FractionalBits),
			4398046511103L >> (63 - FP.FractionalBits),
			2199023255551L >> (63 - FP.FractionalBits),
			1099511627775L >> (63 - FP.FractionalBits),
			549755813887L >> (63 - FP.FractionalBits),
			274877906943L >> (63 - FP.FractionalBits),
			137438953471L >> (63 - FP.FractionalBits),
			68719476736L >> (63 - FP.FractionalBits),
			34359738368L >> (63 - FP.FractionalBits),
			17179869184L >> (63 - FP.FractionalBits),
			8589934592L >> (63 - FP.FractionalBits),
			4294967296L >> (63 - FP.FractionalBits),
			2147483648L >> (63 - FP.FractionalBits),
			1073741824L >> (63 - FP.FractionalBits),
			536870912L >> (63 - FP.FractionalBits),
			268435456L >> (63 - FP.FractionalBits),
			134217728L >> (63 - FP.FractionalBits),
			67108864L >> (63 - FP.FractionalBits),
			33554432L >> (63 - FP.FractionalBits),
			16777216L >> (63 - FP.FractionalBits),
			8388608L >> (63 - FP.FractionalBits),
			4194304L >> (63 - FP.FractionalBits),
			2097152L >> (63 - FP.FractionalBits),
			1048576L >> (63 - FP.FractionalBits),
			524288L >> (63 - FP.FractionalBits),
			262144L >> (63 - FP.FractionalBits),
			131072L >> (63 - FP.FractionalBits),
			65536L >> (63 - FP.FractionalBits),
			32768L >> (63 - FP.FractionalBits),
			16384L >> (63 - FP.FractionalBits),
			8192L >> (63 - FP.FractionalBits),
			4096L >> (63 - FP.FractionalBits),
			2048L >> (63 - FP.FractionalBits),
			1024L >> (63 - FP.FractionalBits),
			512L >> (63 - FP.FractionalBits),
			256L >> (63 - FP.FractionalBits),
			128L >> (63 - FP.FractionalBits),
			64L >> (63 - FP.FractionalBits),
			32L >> (63 - FP.FractionalBits),
			16L >> (63 - FP.FractionalBits),
			8L >> (63 - FP.FractionalBits),
			4L >> (63 - FP.FractionalBits),
			2L >> (63 - FP.FractionalBits),
			1L >> (63 - FP.FractionalBits),
		};
	}
}
