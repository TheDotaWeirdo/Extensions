using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Extensions
{
	public static partial class ExtensionClass
	{
		public delegate void action();

		public delegate void DynamicAction<T>(T t);

		/// <summary>
		/// Adds items from an <see cref="IEnumerable{T2}"/> into the List after a set Conversion
		/// </summary>
		/// <param name="Source">IEnumerable Data Source</param>
		/// <param name="Conversion">Conversion function from T2 to T1</param>
		public static void AddRange<T, T2>(this List<T> list, IEnumerable<T2> Source, Func<T2, T> Conversion)
		{
			if (Source != null)
				foreach (var item in Source)
					list.Add(Conversion(item));
		}

		/// <summary>
		/// Checks if any of the elements in the first <see cref="IEnumerable{T}"/> is equal to any element in the second
		/// </summary>
		public static bool Any<T>(this IEnumerable<T> l, IEnumerable<T> list)
			=> l.Any(x => list.Any(y => x.Equals(y)));

		/// <summary>
		/// Checks if any of the elements in the first <see cref="IEnumerable{T}"/> is equal to one element
		/// </summary>
		public static bool Any<T>(this IEnumerable<T> l, T item)
			=> l.Any(x => x.Equals(item));

		/// <summary>
		/// Checks if any of the elements in the first <see cref="IEnumerable{T}"/> is equal to one element
		/// </summary>
		public static bool Any<T>(this IEnumerable<T> l, params T[] list)
		{
			if (list.Length == 0)
			{
				foreach (var item in l)
					return true;

				return false;
			}

			return list.Any(x => l.Any(item => x.Equals(item)));
		}

		/// <summary>
		/// Converts this <see cref="IEnumerable{T}"/> to an <see cref="IEnumerable{T2}"/> using the conversion <paramref name="func"/>
		/// </summary>
		public static IEnumerable<T2> Convert<T, T2>(this IEnumerable<T> list, Func<T, T2> func)
		{
			var l = new List<T2>(list?.Count() ?? 0);
			l.AddRange(list, func);
			return l.AsEnumerable();
		}

		/// <summary>
		/// Converts this <see cref="MatchCollection"/> to an <see cref="IEnumerable{T}"/> using the conversion <paramref name="func"/>
		/// </summary>
		public static IEnumerable<T> Convert<T>(this MatchCollection list, Func<Match, T> func)
		{
			var l = new List<T>(list.Count);
			foreach (Match item in list)
				l.Add(func(item));
			return l.AsEnumerable();
		}

		/// <summary>
		/// Converts this <see cref="IEnumerable{T}"/> to an <see cref="IEnumerable{T2}"/> using the conversion <paramref name="func"/>
		/// </summary>
		public static IEnumerable<T2> ConvertEnumerable<T, T2>(this IEnumerable<T> list, Func<T, IEnumerable<T2>> func)
		{
			if (list != null)
				foreach (var item in list)
					foreach (var item2 in func(item))
						yield return item2;
		}

		/// <summary>
		/// Cuts out all elements of the <see cref="List{T}"/> outside the selected range
		/// </summary>
		public static void Cut<T>(this List<T> list, int startIndex, int count)
		{
			for (int i = startIndex + count; i < list.Count; i++)
				list.RemoveAt(i);

			for (int i = 0; i < startIndex; i++)
				list.RemoveAt(i);
		}

		public static IEnumerable<T> Distinct<T>(this IEnumerable<T> list, Func<T, T, bool> comparer)
		{
			var items = new List<T>();

			foreach (var item in list)
			{
				if (!items.Any(x => comparer(x, item)))
				{
					items.Add(item);
					yield return item;
				}
			}
		}

		/// <summary>
		/// Returns the first element in the <see cref="IEnumerable{T}"/> that satisfies the <paramref name="predictate"/>
		/// </summary>
		public static T FirstThat<T>(this IEnumerable<T> list, Func<T, bool> predictate)
		{
			if (list != null)
			{
				foreach (var item in list)
					if (predictate(item))
						return item;
			}

			return default(T);
		}

		/// <summary>
		/// Runs an <paramref name="action"/> for each item in the <see cref="IEnumerable{T}"/>
		/// </summary>
		public static void Foreach<T>(this IEnumerable<T> list, DynamicAction<T> action)
		{
			foreach (var item in list)
			{
				action(item);
			}
		}

		/// <summary>
		/// Checks if the <see cref="IEnumerable{T}"/> only contains one element being <paramref name="item"/>
		/// </summary>
		public static bool IsOnly<T>(this IEnumerable<T> list, T item)
			=> list.Count() == 1 && list.First().Equals(item);

		/// <summary>
		/// Returns the last element in the <see cref="IEnumerable{T}"/> that satisfies the <paramref name="predictate"/>
		/// </summary>
		public static T LastThat<T>(this IEnumerable<T> list, Func<T, bool> predictate)
		{
			if (list != null)
			{
				foreach (var item in list.Reverse())
					if (predictate(item))
						return item;
			}

			return default(T);
		}

		public static T Next<T>(this IEnumerable<T> enumerable, T item)
		{
			if (enumerable.Count() == 0)
				return item;

			var list = new List<T>(enumerable);
			var i = list.IndexOf(item);
			if (i + 1 >= list.Count)
				return list.FirstOrDefault();
			return list[i + 1];
		}

		/// <summary>
		/// Removes occurences of the <paramref name="source"/> collection from the <see cref="List{T}"/>
		/// </summary>
		public static void RemoveRange<T>(this List<T> list, IEnumerable<T> source)
		{
			foreach (var item in source)
				list.Remove(item);
		}

		public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
			=> source.Skip(Math.Max(0, source.Count() - count));

		public static IEnumerable<T> Trim<T>(this IEnumerable<T> list, Func<T, bool> comparer)
		{
			var items = new List<T>(list);

			while (items.Any() && comparer(items[0]))
				items.RemoveAt(0);

			while (items.Any() && comparer(items[items.Count - 1]))
				items.RemoveAt(items.Count - 1);

			return items;
		}

		public static IEnumerable<T> Trim<T>(this IEnumerable<T> list, T comparVal)
		{
			var items = new List<T>(list);

			while (items.Any() && comparVal.Equals(items[0]))
				items.RemoveAt(0);

			while (items.Any() && comparVal.Equals(items[items.Count - 1]))
				items.RemoveAt(items.Count - 1);

			return items;
		}

		/// <summary>
		/// Returns the items from the collection within the selected range
		/// </summary>
		public static IEnumerable<T> Trim<T>(this IEnumerable<T> list, int startIndex, int count)
		{
			if (startIndex > list.Count())
				throw new IndexOutOfRangeException();

			var Out = new T[count];

			for (int i = startIndex; i < Math.Min(list.Count(), startIndex + count); i++)
				Out[i - startIndex] = list.ElementAt(i);

			return Out.AsEnumerable();
		}
	}
}