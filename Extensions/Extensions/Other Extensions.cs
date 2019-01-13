using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Extensions
{
	public static partial class ExtensionClass
	{
		private static Random RNG = new Random(Guid.NewGuid().GetHashCode());

		public enum SizeLength { GB, MB, KB, B, bits }

		public static void AddIfNotExist<T>(this List<T> list, T item)
		{
			if (!list.Any(item))
				list.Add(item);
		}

		/// <summary>
		/// Checks if Any of the objects in the <see cref="IEnumerable<T>"/> are equal to the <paramref name="item"/>
		/// </summary>
		public static bool AnyOf<T>(this T item, params T[] List)
			=> List.Any(x => x.Equals(item));

		/// <summary>
		/// Checks if All of the objects in the <see cref="IEnumerable<T>"/> are equal to the <paramref name="item"/>
		/// </summary>
		public static bool AllOf<T>(this T item, params T[] List)
			=> List.All(x => x.Equals(item));

		public static IEnumerable<Match> AsEnumerable(this MatchCollection matches)
		{
			foreach (Match item in matches)
				yield return item;
		}

		/// <summary>
		/// Returns <paramref name="changeValue"/> if this <see cref="{T}"/> is equal to the <paramref name="compareValue"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="compareValue">Value to compare with</param>
		/// <param name="changeValue">Value returned if the comparison is successful</param>
		public static T If<T>(this T baseValue, T compareValue, T changeValue)
			=> baseValue.Equals(compareValue) ? changeValue : baseValue;

		public static T If<T>(this T baseValue, bool condition, T changeValue)
			=> condition ? changeValue : baseValue;

		public static T If<T>(this T baseValue, bool condition, Func<T, T> changeValue)
			=> condition ? changeValue(baseValue) : baseValue;

		/// <summary>
		/// Returns <paramref name="changeValue"/> if this <see cref="{T}"/> is equal to the <paramref name="compareValue"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="compareValue">Value to compare with</param>
		/// <param name="changeValue">Value returned if the comparison is successful</param>
		public static T IfNull<T>(this T baseValue, T changeValue)
			=> baseValue == null ? changeValue : baseValue;

		public static T IfNull<T, T2>(this T2 baseValue, T changeValue, T elseValue)
			=> baseValue == null ? changeValue : elseValue;

		/// <summary>
		/// Returns <paramref name="changeValue"/> if the <paramref name="compareFunc"/> returns <see cref="true"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="compareFunc">Method used to test the base <see cref="{T}"/></param>
		/// <param name="changeValue">Value returned if the comparison is successful</param>
		public static T If<T>(this T baseValue, Func<T, bool> compareFunc, T changeValue)
			=> compareFunc(baseValue) ? changeValue : baseValue;

		/// <summary>
		/// Returns <paramref name="changeValue"/> if this <see cref="{T}"/> is equal to the <paramref name="compareValue"/>, else returns <paramref name="elseValue"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="compareValue">Value to compare with</param>
		/// <param name="changeValue">Value returned if the comparison is successful</param>
		public static T2 If<T, T2>(this T baseValue, T compareValue, T2 changeValue, T2 elseValue)
			=> baseValue.Equals(compareValue) ? changeValue : elseValue;

		/// <summary>
		/// Returns <paramref name="changeValue"/> if this <see cref="{T}"/> is equal to the <paramref name="compareValue"/>, else returns <paramref name="elseValue"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="compareValue">Value to compare with</param>
		/// <param name="changeValue">Value returned if the comparison is successful</param>
		public static T If<T>(this bool baseValue, T changeValue, T elseValue)
			=> baseValue ? changeValue : elseValue;

		/// <summary>
		/// Returns <paramref name="changeValue"/> if the <paramref name="compareFunc"/> returns <see cref="true"/>, else returns <paramref name="elseValue"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="compareFunc">Method used to test the base <see cref="{T}"/></param>
		/// <param name="changeValue">Value returned if the comparison is successful</param>
		/// <param name="elseValue">Value returned if the comparison was unsuccessful</param>
		public static T2 If<T, T2>(this T baseValue, Func<T, bool> compareFunc, T2 changeValue, T2 elseValue)
			=> compareFunc(baseValue) ? changeValue : elseValue;

		/// <summary>
		/// Returns <paramref name="changeValue"/> if the <paramref name="compareFunc"/> returns <see cref="true"/>, else returns <paramref name="elseValue"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="compareFunc">Method used to test the base <see cref="{T}"/></param>
		/// <param name="changeValue">Value returned if the comparison is successful</param>
		/// <param name="elseValue">Value returned if the comparison was unsuccessful</param>
		public static T2 If<T, T2>(this T baseValue, Func<T, bool> compareFunc, T2 changeValue, Func<T, T2> elseValue)
			=> compareFunc(baseValue) ? changeValue : elseValue(baseValue);

		/// <summary>
		/// Returns <paramref name="changeValue"/> if the <paramref name="compareFunc"/> returns <see cref="true"/>, else returns <paramref name="elseValue"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="compareFunc">Method used to test the base <see cref="{T}"/></param>
		/// <param name="changeValue">Value returned if the comparison is successful</param>
		/// <param name="elseValue">Value returned if the comparison was unsuccessful</param>
		public static T2 If<T, T2>(this T baseValue, Func<T, bool> compareFunc, Func<T, T2> changeValue, T2 elseValue)
			=> compareFunc(baseValue) ? changeValue(baseValue) : elseValue;

		/// <summary>
		/// Returns <paramref name="changeValue"/> if the <paramref name="compareFunc"/> returns <see cref="true"/>, else returns <paramref name="elseValue"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="compareFunc">Method used to test the base <see cref="{T}"/></param>
		/// <param name="changeValue">Value returned if the comparison is successful</param>
		/// <param name="elseValue">Value returned if the comparison was unsuccessful</param>
		public static T2 If<T, T2>(this T baseValue, Func<T, bool> compareFunc, Func<T, T2> changeValue, Func<T, T2> elseValue)
			=> compareFunc(baseValue) ? changeValue(baseValue) : elseValue(baseValue);

		/// <summary>
		/// Returns a random item in the <see cref="List{T}"/>
		/// </summary>
		public static T Random<T>(this List<T> L)
			=> L[RNG.Next(L.Count)];

		/// <summary>
		/// Returns a random item in the <see cref="Array"/>
		/// </summary>
		public static T Random<T>(this T[] A)
			=> A[RNG.Next(A.Length)];

		/// <summary>
		/// Gets the Data Size in the <see cref="SizeLength"/> format, base size must be in Bytes
		/// </summary>
		public static double Size(this long size, SizeLength sizeLength)
		{
			switch (sizeLength)
			{
				case SizeLength.GB:
					return Math.Round(size / 1073741824d, 2);

				case SizeLength.MB:
					return Math.Round(size / 1048576d, 2);

				case SizeLength.KB:
					return Math.Round(size / 1024d, 2);

				case SizeLength.bits:
					return Math.Round(size * 8d, 2);

				default:
					return size;
			}
		}

		/// <summary>
		/// Swaps 2 variable values
		/// </summary>
		public static void Swap<T>(ref T O1, ref T O2)
		{ T O = O1; O1 = O2; O2 = O; }

		public static IEnumerable<T2> ThatAre<T2, T>(this IEnumerable<T> list) where T2 : T
		{
			var l = new List<T2>();

			foreach (var item in list)
				if (item is T2)
					l.Add((T2)item);

			return l;
		}

		public static IEnumerable<T> ThatAre<T>(this Control.ControlCollection list) where T : Control
		{
			var l = new List<T>();

			foreach (var item in list)
				if (item is T)
					l.Add((T)item);

			return l;
		}

		public static string YesNo(this bool b)
			=> b ? "Yes" : "No";
	}
}