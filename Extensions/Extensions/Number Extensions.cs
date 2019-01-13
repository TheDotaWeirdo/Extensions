using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Extensions
{
	public static partial class ExtensionClass
	{
		private static Dictionary<char, int> RomanMap = new Dictionary<char, int>()
		{ {'I', 1}, {'V', 5}, {'X', 10}, {'L', 50}, {'C', 100}, {'D', 500}, {'M', 1000} };

		/// <summary>
		/// Returns 1 if true, 0 if false
		/// </summary>
		public static int AsBinaryInt(this bool b) => b ? 1 : 0;

		/// <summary>
		/// Returns "1" if true, "0" if false
		/// </summary>
		public static string AsBinaryString(this bool b) => b ? "1" : "0";

		/// <summary>
		/// Forces the <see cref="int"/> to stay between two values
		/// </summary>
		public static int Between(this int obj, int Min, int Max)
			=> Math.Min(Max, Math.Max(Min, obj));

		/// <summary>
		/// Forces the <see cref="double"/> to stay between two values
		/// </summary>
		public static double Between(this double obj, double Min, double Max)
			=> Math.Min(Max, Math.Max(Min, obj));

		/// <summary>
		/// Forces the <see cref="float"/> to stay between two values
		/// </summary>
		public static float Between(this float obj, float Min, float Max)
			=> Math.Min(Max, Math.Max(Min, obj));

		/// <summary>
		/// Checks if the value is within the set range
		/// </summary>
		public static bool IsWithin(this double d, double Min, double Max)
																																					=> d > Min && d < Max;

		/// <summary>
		/// Checks if the value is within the set range
		/// </summary>
		public static bool IsWithin(this float d, float Min, float Max)
			=> d > Min && d < Max;

		/// <summary>
		/// Checks if the value is within the set range
		/// </summary>
		public static bool IsWithin(this int d, int Min, int Max)
			=> d > Min && d < Max;

		/// <summary>
		/// Converts an <see cref="int"/> into a Roman <see cref="string"/>
		/// </summary>
		public static int RomanToInteger(this string roman)
		{
			roman = Regex.Match(roman, "[IVXLCDM]+").Value;
			int number = 0;
			for (int i = 0; i < roman.Length; i++)
			{
				if (i + 1 < roman.Length && RomanMap[roman[i]] < RomanMap[roman[i + 1]])
					number -= RomanMap[roman[i]];
				else
					number += RomanMap[roman[i]];
			}
			return number;
		}

		/// <summary>
		/// Returns 1 if positive, -1 if negative or else returns 0
		/// </summary>
		public static int Sign(this int i)
			=> i > 0 ? 1 : i < 0 ? -1 : 0;

		/// <summary>
		/// Returns 1 if positive, -1 if negative or else returns 0
		/// </summary>
		public static int Sign(this double i)
			=> i > 0 ? 1 : i < 0 ? -1 : 0;
	}
}