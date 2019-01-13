using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Extensions
{
	public static class ExtensionClass
	{
		#region String Extensions

		/// <summary>
		/// Checks if either one of the strings are Abbreviations of the other
		/// </summary>
		public static bool AbbreviationCheck(this string string1, string string2)
		{
			if (string.IsNullOrWhiteSpace(string1) || string.IsNullOrWhiteSpace(string1))
				return false;

			string1 = string1.ToLower().Replace("'s ", " ");
			string2 = string2.ToLower().Replace("'s ", " ");

			return string1.Where(x => x != ' ') == string2.GetAbbreviation() || string1.GetAbbreviation() == string2.Where(x => x != ' ');
		}

		/// <summary>
		/// Gets what's between the <paramref name="Start"/> <see cref="char"/> and the <paramref name="End"/> <see cref="string"/> in the given <see cref="string"/>
		/// </summary>
		public static string Between(this string S, char Start, string End)
			=> S.Between(Start.ToString(), End);

		/// <summary>
		/// Gets what's between the <paramref name="Start"/> <see cref="string"/> and the <paramref name="End"/> <see cref="char"/> in the given <see cref="string"/>
		/// </summary>
		public static string Between(this string S, string Start, char End)
			=> S.Between(Start, End.ToString());

		/// <summary>
		/// Gets what's between the <paramref name="Start"/> <see cref="char"/> and the <paramref name="End"/> <see cref="char"/> in the given <see cref="string"/>
		/// </summary>
		public static string Between(this string S, char Start, char End)
			=> S.Between(Start.ToString(), End.ToString());

		/// <summary>
		/// Gets what's between the <paramref name="Start"/> <see cref="string"/> and the <paramref name="End"/> <see cref="string"/> in the given <see cref="string"/>
		/// </summary>
		/// <param name="IgnoreCase">Ignores case sensitivity</param>
		public static string Between(this string S, string Start, string End, bool IgnoreCase = false)
		{
			var RX = IgnoreCase ? new Regex($@"(?<={Regex.Escape(Start)}).+?(?={Regex.Escape(End)})", RegexOptions.IgnoreCase) : new Regex($@"(?<={Regex.Escape(Start)}).+?(?={Regex.Escape(End)})");
			return RX.IsMatch(S) ? RX.Match(S).Value : S;
		}

		/// <summary>
		/// Checks if the <see cref="string"/> contains a specific Word
		/// </summary>
		public static bool ContainsWord(this string S, string Word)
			=> Regex.IsMatch(S, @"\b" + Regex.Escape(Word) + @"\b");

		public static string Copy(this string str, int times)
		{
			var sb = new StringBuilder(str.Length * times);

			for (int i = 0; i < times; i++)
				sb.Append(str);

			return sb.ToString();
		}

		/// <summary>
		/// Gets the Abbreviation of the <see cref="string"/>
		/// </summary>
		public static string GetAbbreviation(this string S)
		{
			var SB = new StringBuilder();
			foreach (var item in S.GetWords(true))
				SB.Append(item.All(x => char.IsDigit(x)) ? item : item[0].ToString());

			if (Regex.IsMatch(SB.ToString(), "^[A-z]+[0-9]+$"))
			{
				var match = Regex.Match(SB.ToString(), "^([A-z]+)([0-9]+)$");
				return $"{match.Groups[1]} {match.Groups[2]}";
			}

			return SB.ToString();
		}

		/// <summary>
		/// Gets an <see cref="string[]"/> containing all the Words in the <see cref="string"/>
		/// </summary>
		public static string[] GetWords(this string S, bool includeNumbers = false)
			=> (S == string.Empty) ? new string[0] : Regex.Matches(S, @"\b" + (includeNumbers ? "" : "(?![0-9])") + @"(\w+)(?:'\w+)?\b")
				.Convert(x => x.Groups[1].Value).ToArray();

		public static string FormatWords(this string str, bool forceUpper = false)
		{
			str = str.RegexReplace("([a-z])([A-Z])", x => $"{x.Groups[1].Value} {x.Groups[2].Value}", false);

			if (forceUpper)
				str = str.RegexReplace(@"(^[a-z])|(\ [a-z])", x => x.Value.ToUpper());

			return str;
		}

		/// <summary>
		/// Changes the value of the string to <paramref name="value"/> if the string is null or white space
		/// </summary>
		public static string IfEmpty(this string S, string value)
			=> string.IsNullOrWhiteSpace(S) ? value : S;

		/// <summary>
		/// Lists all the strings in the <see cref="IEnumerable{string}"/> next to each other in one <see cref="string"/>
		/// </summary>
		public static string ListStrings(this IEnumerable<string> list) => list.ListStrings(x => x);

		public static string ListStrings<T>(this IEnumerable<T> list, string seperator)
			=> list.ListStrings(x => $"{x}{seperator}", false);

		/// <summary>
		/// Lists all the strings in the <see cref="IEnumerable{string}"/> with a set <paramref name="Format"/>
		/// </summary>
		/// <param name="Format">Format to change each string, null leaves the strings as they are</param>
		public static string ListStrings<T>(this IEnumerable<T> list, Func<T, string> Format, bool applyToLast = true)
		{
			if (list == null)
				return string.Empty;

			var SB = new StringBuilder();

			for (int i = 0; i < list.Count(); i++)
			{
				if (!applyToLast && i == list.Count() - 1)
					SB.Append(list.ElementAt(i));
				else
					SB.Append(Format(list.ElementAt(i)));
			}

			return SB.ToString();
		}

		/// <summary>
		/// Removes all occurrence of a <see cref="Regex"/> <paramref name="pattern"/> in the <see cref="string"/>
		/// </summary>
		/// <param name="pattern"><see cref="Regex"/> Pattern</param>
		/// <param name="group">Specifies which <paramref name="group"/> in the <see cref="Regex"/> match to remove</param>
		/// <param name="ignoreCase">IgnoreCase <see cref="Regex"/> option</param>
		public static string RegexRemove(this string @base, string pattern, int group = 0, bool ignoreCase = true)
			=> ignoreCase ? Regex.Replace(@base, pattern, x => x.Value.Remove(x.Groups[group].Value), RegexOptions.IgnoreCase) : Regex.Replace(@base, pattern, x => x.Value.Remove(x.Groups[group].Value));

		/// <summary>
		/// Replaces all occurrence of a <see cref="Regex"/> <paramref name="pattern"/> with a <paramref name="replacement"/>
		/// </summary>
		/// <param name="pattern"><see cref="Regex"/> Pattern</param>
		/// <param name="replacement">Text Replacement</param>
		/// <param name="ignoreCase">IgnoreCase <see cref="Regex"/> option</param>
		public static string RegexReplace(this string @base, string pattern, string replacement = "", bool ignoreCase = true)
			=> ignoreCase ? Regex.Replace(@base, pattern, replacement, RegexOptions.IgnoreCase) : Regex.Replace(@base, pattern, replacement);

		/// <summary>
		/// Replaces all occurrence of a <see cref="Regex"/> <paramref name="pattern"/> with the result of the <paramref name="replacement"/> method
		/// </summary>
		/// <param name="pattern"><see cref="Regex"/> Pattern</param>
		/// <param name="replacement">Method that converts the matched string</param>
		/// <param name="ignoreCase">IgnoreCase <see cref="Regex"/> option</param>
		public static string RegexReplace(this string @base, string pattern, MatchEvaluator replacement = null, bool ignoreCase = true)
			=> ignoreCase ? Regex.Replace(@base, pattern, replacement, RegexOptions.IgnoreCase) : Regex.Replace(@base, pattern, replacement);

		/// <summary>
		/// Removes the first occurrence of the text in the <see cref="string"/>
		/// </summary>
		public static string Remove(this string S, string text)
			=> S.IndexOf(text) >= 0 ? S.Remove(S.IndexOf(text), text.Length) : S;

		/// <summary>
		/// Removes the character at the specified index
		/// </summary>
		public static string RemoveAt(this string S, int index)
			=> S.Remove(index, 1);

		/// <summary>
		/// Removes any multiple spaces after each other and any starting or ending spaces
		/// </summary>
		public static string RemoveDoubleSpaces(this string S)
			=> S.RegexRemove(@" {2,}").RegexRemove(@"^ ").RegexRemove(@" $");

		/// <summary>
		/// Parses the <see cref="string"/> into an <see cref="int"/> disregarding non-digit characters
		/// </summary>
		/// <returns>Returns 0 if no digits are found</returns>
		public static int SmartParse(this string S, int defaultValue = 0)
			=> !string.IsNullOrWhiteSpace(S) && (int.TryParse(S.Where(c => char.IsDigit(c)), out var i)) ? i : defaultValue;

		/// <summary>
		/// Checks if the <see cref="string"/> matches <paramref name="s2"/> with a <paramref name="MaxErrors"/> margin
		/// </summary>
		/// <param name="s2"><see cref="string"/> to match against</param>
		/// <param name="maxErrors">Maximum amount of differences between the two strings</param>
		/// <param name="caseCheck">Option to match the strings with Case Sensitivity</param>
		/// <returns></returns>
		public static bool SpellCheck(this string s1, string s2, int maxErrors = 2, bool caseCheck = true) 
			=> SpellCheck(s1, s2, caseCheck) <= maxErrors;

		/// <summary>
		/// Returns the total amount of differences between the two strings
		/// </summary>
		/// <param name="S2"><see cref="string"/> to match against</param>
		/// <param name="caseCheck">Option to match the strings with Case Sensitivity</param>
		public static int SpellCheck(this string s1, string s2, bool caseCheck = true)
		{
			if(!caseCheck)
			{
				s1 = s1.ToLower();
				s2 = s2.ToLower();
			}

			// Levenshtein Algorithm
			var n = s1.Length;
			var m = s2.Length;
			var d = new int[n + 1, m + 1];

			if (n == 0)
				return m;

			if (m == 0)
				return n;

			for (int i = 0; i <= n; d[i, 0] = i++) { }

			for (int j = 0; j <= m; d[0, j] = j++) { }

			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= m; j++)
				{
					int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;

					d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						 d[i - 1, j - 1] + cost);
				}
			}
			return d[n, m];
		}

		/// <summary>
		/// Changes the <see cref="string"/> to a Capitalize Each Word <see cref="string"/>
		/// </summary>
		/// <param name="ForceLowerCase">Forces all existing upper-case into lower-case before conversion</param>
		public static string ToCapital(this string S, bool ForceLowerCase = true)
			=> Regex.Replace(ForceLowerCase ? S.ToLower() : S, @"\b(([A-z])(\w+)?)", new MatchEvaluator(x => ToUpper(x, S)));

		/// <summary>
		/// Changes all strings in the <see cref="IEnumerable{string}"/> to Lowercase
		/// </summary>
		public static IEnumerable<string> ToLower(this IEnumerable<string> IE)
		{
			var array = IE.ToArray();

			for (int i = 0; i < array.Count(); i++)
				array[i] = array[i].ToLower();

			return array.AsEnumerable();
		}

		/// <summary>
		/// Removes <paramref name="count"/> characters from the end of a <see cref="string"/>
		/// </summary>
		public static string TrimEnd(this string s, int count)
																									=> s.Length >= count ? s.Substring(0, s.Length - count) : string.Empty;

		/// <summary>
		/// Filters the <see cref="string"/> based on the <paramref name="Test"/> for each <see cref="char"/>
		/// </summary>
		/// <param name="S"></param>
		/// <param name="Test"></param>
		/// <returns></returns>
		public static string Where(this string S, Func<char, bool> Test)
		{
			var Out = new StringBuilder(S.Length);
			foreach (var c in S)
				if (Test(c))
					Out.Append(c);
			return Out.ToString();
		}

		/// <summary>
		/// Changes all strings in the <see cref="IEnumerable{string}"/> to Uppercase
		/// </summary>
		private static string ToUpper(Match match, string Base)
		{
			var low = match.Value.ToLower();
			if (!(match.Index > 0 && char.IsLetter(low[0]) && Base[match.Index - 1] == '\'') &&
				((match.Index == 0) || (!(low == "the" || low == "in" || low == "of" || low == "at" || low == "and"))))
				return match.Value[0].ToString().ToUpper() + (match.Length == 0 ? "" : match.Value.Substring(1));
			return match.Value;
		}

		#endregion String Extensions

		#region Enumerable Extensions

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
			=> list.Any(x => l.Any(item => x.Equals(item)));

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
			var Out = new List<T2>();
			if (list != null)
				foreach (var item in list)
				{
					var res = func(item);
					if (res != null)
						Out.AddRange(res);
				}
			return Out;
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

		#endregion Enumerable Extensions

		#region Dictionary Extensions

		/// <summary>
		/// Adds the <paramref name="valuePair"/> to the <see cref="Dictionary{T, T2}"/>
		/// </summary>
		public static void Add<T, T2>(this Dictionary<T, T2> dic, KeyValuePair<T, T2> valuePair)
		{ if (!dic.ContainsKey(valuePair.Key)) dic.Add(valuePair.Key, valuePair.Value); }

		/// <summary>
		/// Adds the <see cref="IEnumerable{KeyValuePair}"/> <paramref name="valuePair"/> to the <see cref="Dictionary{T, T2}"/>
		/// </summary>
		public static void AddRange<T, T2>(this Dictionary<T, T2> dictionary, IEnumerable<KeyValuePair<T, T2>> valuesPairs)
		{
			foreach (var item in valuesPairs)
				dictionary.Add(item);
		}

		/// <summary>
		/// Converts this collection of <see cref="KeyValuePair{TK, TV}"/> to a <see cref="Dictionary{TK, TV}"/>
		/// </summary>
		public static Dictionary<TK, TV> AsDictionary<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> list)
			=> list.ConvertDictionary(x => x);

		/// <summary>
		/// Converts this <see cref="IEnumerable{T}"/> to a <see cref="Dictionary{TK, TV}"/> using the conversion <paramref name="func"/>
		/// </summary>
		public static Dictionary<TK, TV> ConvertDictionary<T, TK, TV>(this IEnumerable<T> list, Func<T, KeyValuePair<TK, TV>> func)
		{
			var dic = new Dictionary<TK, TV>();
			dic.AddRange(list.Convert(func));
			return dic;
		}

		/// <summary>
		/// Adds the specified key and value to the dictionary or updates the key with the new value
		/// </summary>
		public static void TryAdd<TK, TV>(this Dictionary<TK,TV> dic, TK key, TV val)
		{
			if (dic.ContainsKey(key))
				dic[key] = val;
			else
				dic.Add(key, val);
		}

		#endregion Dictionary Extensions

		#region Form Extensions

		private const int WM_SETREDRAW = 11;
		private static bool? isadministrator;

		/// <summary>
		/// Checks if the App currently has Administrator Privileges
		/// </summary>
		public static bool IsAdministrator => (bool)(isadministrator ??
			(isadministrator = new WindowsPrincipal(WindowsIdentity.GetCurrent())
				.IsInRole(WindowsBuiltInRole.Administrator)));

		/// <summary>
		/// Associates a <see cref="ToolTip"/> with a <paramref name="control"/> and all of its children
		/// </summary>
		public static void AdvancedSetTooltip(this ToolTip toolTip, Control control, string tip)
		{
			toolTip.SetToolTip(control, tip);

			foreach (Control child in control.Controls)
				AdvancedSetTooltip(toolTip, child, tip);
		}

		/// <summary>
		/// Removes controls from the collection
		/// </summary>
		/// <param name="dispose">Completely Dispose of the controls</param>
		/// <param name="testFunc">Only remove controls that satisfy this method</param>
		public static void Clear(this Control.ControlCollection controls, bool dispose, Func<Control, bool> testFunc = null)
		{
			for (int ix = controls.Count - 1; ix >= 0; --ix)
			{
				if (testFunc == null || testFunc(controls[ix]))
				{
					if (dispose)
						controls[ix].Dispose();
					else
						controls.RemoveAt(ix);
				}
			}
		}

		/// <summary>
		/// Colors the <see cref="Image"/> of a <see cref="PictureBox"/> with the <paramref name="color"/>
		/// </summary>
		public static void Color(this PictureBox pictureBox, Color color)
			=> pictureBox.Image = pictureBox.Image.Color(color);

		/// <summary>
		/// Colors the <see cref="Image"/> with the <paramref name="color"/>
		/// </summary>
		public static Bitmap Color(this Image image, Color color) => (image as Bitmap).Color(color);

		/// <summary>
		/// Colors the <see cref="Bitmap"/> with the <paramref name="color"/>
		/// </summary>
		public static Bitmap Color(this Bitmap bitmap, Color color)
		{
			if (bitmap == null) return null;
			try
			{
				var W = bitmap.Width;
				var H = bitmap.Height;

				for (int i = 0; i < H; i++)
				{
					for (int j = 0; j < W; j++)
					{
						bitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(bitmap.GetPixel(j, i).A, color));
					}
				}

				return bitmap;
			}
			catch { return null; }
		}

		/// <summary>
		/// Creates a <see cref="System.Drawing.Color"/> from a Hue, Saturation and Luminance combination
		/// </summary>
		public static Color ColorFromHSL(double h, double s, double l)
		{
			double r = 0, g = 0, b = 0;
			if (l != 0)
			{
				if (s == 0)
					r = g = b = l;
				else
				{
					double temp2;
					if (l < 0.5)
						temp2 = l * (1.0 + s);
					else
						temp2 = l + s - (l * s);

					double temp1 = 2.0 * l - temp2;

					r = GetColorComponent(temp1, temp2, h + 1.0 / 3.0);
					g = GetColorComponent(temp1, temp2, h);
					b = GetColorComponent(temp1, temp2, h - 1.0 / 3.0);
				}
			}
			return System.Drawing.Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
		}

		/// <summary>
		/// Converts this <see cref="ListBox.SelectedObjectCollection"/> to an <see cref="IEnumerable{T}"/> using the conversion <paramref name="func"/>
		/// </summary>
		public static IEnumerable<T> Convert<T>(this ListBox.SelectedObjectCollection list, Func<object, T> func)
		{
			var l = new List<T>(list.Count);
			foreach (var item in list)
				l.Add(func(item));
			return l.AsEnumerable();
		}

		public static bool TryInvoke(this Control control, action action)
		{
			try
			{
				if (control.InvokeRequired)
					control.Invoke(new Action(action));
				else
					action();
			}
			catch { return false; }

			return true;
		}

		/// <summary>
		/// Checks if a <see cref="Control"/> is ultimately visible to the User
		/// </summary>
		public static bool IsVisible(this Control control)
			=> control.Visible && (control.Parent == null || IsVisible(control.Parent));

		/// <summary>
		/// Merges two Colors together
		/// </summary>
		public static Color MergeColor(this Color color, Color backColor, int Perc = 50)
		{
			if (backColor == null) return color;
			var R = (color.R * Perc / 100) + (backColor.R * (100 - Perc) / 100);
			var G = (color.G * Perc / 100) + (backColor.G * (100 - Perc) / 100);
			var B = (color.B * Perc / 100) + (backColor.B * (100 - Perc) / 100);
			return System.Drawing.Color.FromArgb(R, G, B);
		}

		/// <summary>
		/// Sorts the controls of a panel using a <paramref name="keySelector"/>
		/// </summary>
		public static void OrderBy<TKey>(this Panel panel, Func<Control, TKey> keySelector, bool suspendDrawing = true)
		{
			var controls = new List<Control>();

			foreach (Control item in panel.Controls)
				controls.Add(item);

			var index = 0;
			if (suspendDrawing)
				panel.SuspendDrawing();
			foreach (var item in controls.OrderBy(keySelector))
				panel.Controls.SetChildIndex(item, index++);
			if (suspendDrawing)
				panel.ResumeDrawing();
		}

		/// <summary>
		/// Sorts the controls of a panel using a <paramref name="keySelector"/> in a descending order
		/// </summary>
		public static void OrderByDescending<TKey>(this Panel panel, Func<Control, TKey> keySelector, bool suspendDrawing = true)
		{
			if (panel == null || panel.Controls == null)
				return;

			var controls = new List<Control>();

			foreach (Control item in panel.Controls)
				controls.Add(item);

			var index = 0;
			if (suspendDrawing)
				panel.SuspendDrawing();
			foreach (var item in controls.OrderByDescending(keySelector))
				panel.Controls.SetChildIndex(item, index++);
			if (suspendDrawing)
				panel.ResumeDrawing();
		}

		public static void RecursiveClick(this Control control, EventHandler handler)
		{
			control.Click += handler;

			foreach (Control child in control.Controls)
				RecursiveClick(child, handler);
		}

		/// <summary>
		/// Resumes the Drawing of a <see cref="Control"/>
		/// </summary>
		public static void ResumeDrawing(this Control parent)
		{
			try
			{
				SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
				parent.Refresh();
				parent.ResumeLayout(true);
			}
			catch { }
		}

		private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
		{
			int diameter = radius * 2;
			Size size = new Size(diameter, diameter);
			Rectangle arc = new Rectangle(bounds.Location, size);
			GraphicsPath path = new GraphicsPath();

			if (radius == 0)
			{
				path.AddRectangle(bounds);
				return path;
			}

			// top left arc  
			path.AddArc(arc, 180, 90);

			// top right arc  
			arc.X = bounds.Right - diameter;
			path.AddArc(arc, 270, 90);

			// bottom right arc  
			arc.Y = bounds.Bottom - diameter;
			path.AddArc(arc, 0, 90);

			// bottom left arc 
			arc.X = bounds.Left;
			path.AddArc(arc, 90, 90);

			path.CloseFigure();
			return path;
		}

		public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
		{
			if (graphics == null)
				throw new ArgumentNullException("graphics");
			if (pen == null)
				throw new ArgumentNullException("pen");

			using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
			{
				graphics.DrawPath(pen, path);
			}
		}

		public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
		{
			if (graphics == null)
				throw new ArgumentNullException("graphics");
			if (brush == null)
				throw new ArgumentNullException("brush");

			using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
			{
				graphics.FillPath(brush, path);
			}
		}

		/// <summary>
		/// Rotates the <see cref="Bitmap"/> using the <paramref name="flipType"/>
		/// </summary>
		public static Bitmap Rotate(this Bitmap bitmap, RotateFlipType flipType = RotateFlipType.Rotate90FlipNone)
		{
			if (bitmap == null) return null;
			bitmap.RotateFlip(flipType);
			return bitmap;
		}

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

		/// <summary>
		/// Shows, Brings to Front and Restores the <see cref="Form"/>
		/// </summary>
		public static T ShowUp<T>(this T form, bool initialize) where T : Form, new()
		{
			if (initialize && (form == null || form.IsDisposed))
				form = new T();

			if (form.WindowState == FormWindowState.Minimized)
				form.WindowState = FormWindowState.Normal;

			form.Show();
			form.BringToFront();

			return form;
		}

		/// <summary>
		/// Shows, Brings to Front and Restores the <see cref="Form"/>
		/// </summary>
		public static void ShowUp(this Form form) 
		{
			if (form.WindowState == FormWindowState.Minimized)
				form.WindowState = FormWindowState.Normal;

			form.Show();
			form.BringToFront();
		}

		/// <summary>
		/// Stops all Draw events of a <see cref="Control"/>, use <see cref="ResumeDrawing(Control)"/> to revert
		/// </summary>
		public static void SuspendDrawing(this Control parent)
		{
			try
			{
				SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
				parent.SuspendLayout();
			}
			catch { }
		}

		/// <summary>
		/// Tints the <see cref="Bitmap"/> with selected Luminance, Saturation and Hue
		/// </summary>
		/// <param name="Lum">Added Luminance, ranges from -100 to 100</param>
		/// <param name="Sat">Added Saturation, ranges from -100 to 100</param>
		/// <param name="Hue">Added Hue, ranges from -360 to 360</param>
		public static Bitmap Tint(this Bitmap bitmap, float Lum = 0, float Sat = 0, float Hue = 0)
		{
			if (bitmap == null) return null;
			var W = bitmap.Width;
			var H = bitmap.Height;
			double nH, nS, nL;

			for (int i = 0; i < H; i++)
			{
				for (int j = 0; j < W; j++)
				{
					var color = bitmap.GetPixel(j, i);
					nH = ((color.GetHue() + Hue) / 360d).Between(0, 1);
					nS = (color.GetSaturation() + (Sat / 100d)).Between(0, 1);
					nL = (color.GetBrightness() + (Lum / 100d)).Between(0, 1);

					bitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(bitmap.GetPixel(j, i).A,
						ColorFromHSL(nH, nS, nL)));
				}
			}

			return bitmap;
		}

		/// <summary>
		/// Tints a <see cref="Color"/> using the Hue of a <paramref name="source"/> <see cref="Color"/>, Luminance and Saturation
		/// </summary>
		/// <param name="Lum">Added Luminance, ranges from -100 to 100</param>
		/// <param name="Sat">Added Saturation, ranges from -100 to 100</param>
		public static Color Tint(this Color color, Color source, float Lum = 0, float Sat = 0)
			=> color.Tint(source.GetHue(), Lum, Sat);

		/// <summary>
		/// Tints the <see cref="Color"/> with selected Luminance, Saturation and Hue
		/// </summary>
		/// <param name="Lum">Added Luminance, ranges from -100 to 100</param>
		/// <param name="Sat">Added Saturation, ranges from -100 to 100</param>
		/// <param name="Hue">Added Hue, ranges from -360 to 360</param>
		public static Color Tint(this Color color, float? Hue = null, float Lum = 0, float Sat = 0)
			=> ColorFromHSL((double)(Hue ?? color.GetHue()) / 360, (color.GetSaturation() + (Sat / 100d)).Between(0, 1), (color.GetBrightness() + (Lum / 100d)).Between(0, 1));

		/// <summary>
		/// Returns a collection of the controls that match the <paramref name="test"/>
		/// </summary>
		public static IEnumerable<Control> Where(this Control.ControlCollection controls, Func<Control, bool> test)
		{
			var Out = new List<Control>();
			foreach (var item in controls)
				if (test(item as Control))
					Out.Add(item as Control);
			return Out.AsEnumerable();
		}

		private static double GetColorComponent(double temp1, double temp2, double temp3)
		{
			if (temp3 < 0.0)
				temp3 += 1.0;
			else if (temp3 > 1.0)
				temp3 -= 1.0;

			if (temp3 < 1.0 / 6.0)
				return temp1 + (temp2 - temp1) * 6.0 * temp3;
			else if (temp3 < 0.5)
				return temp2;
			else if (temp3 < 2.0 / 3.0)
				return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
			else
				return temp1;
		}

		private static float HuetoRGB(float p, float q, float t)
		{
			if (t < 0) t += 1;
			if (t > 1) t -= 1;
			if (t < 1 / 6) return p + (q - p) * 6 * t;
			if (t < 1 / 2) return q;
			if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
			return p;
		}

		#endregion Form Extensions

		#region I/O Extensions

		/// <summary>
		/// Creates a windows Shortcut (.lnk)
		/// </summary>
		/// <param name="Shortcut">Path of the Shortcut to create</param>
		/// <param name="TargetPath">Reference Path of the Shortcut</param>
		public static void CreateShortcut(string Shortcut, string TargetPath)
		{
			Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); //Windows Script Host Shell Object
			dynamic shell = Activator.CreateInstance(t);
			try
			{
				var lnk = shell.CreateShortcut(Shortcut);
				try
				{
					lnk.TargetPath = TargetPath;
					lnk.Save();
				}
				finally
				{
					Marshal.FinalReleaseComObject(lnk);
				}
			}
			finally
			{
				Marshal.FinalReleaseComObject(shell);
			}
		}

		/// <summary>
		/// Creates a Shortcut for the file at the <paramref name="shortcutPath"/>
		/// </summary>
		public static void CreateShortcut(this FileInfo file, string shortcutPath)
			=> CreateShortcut(shortcutPath, file.FullName);

		/// <summary>
		/// Returns the Name of the file without its Extension
		/// </summary>
		public static string FileName(this FileInfo file)
				=> file.Name.Substring(0, file.Name.LastIndexOf(file.Extension));

		/// <summary>
		/// Returns all Directories in the path within the selected <paramref name="layers"/>
		/// </summary>
		public static string[] GetDirectories(string path, string pattern, int layers)
		{
			if (layers == 0)
				return new string[0];

			var Out = new List<string>();
			Out.AddRange(Directory.GetDirectories(path, pattern, SearchOption.TopDirectoryOnly));

			foreach (var item in Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly))
				Out.AddRange(GetDirectories(item, pattern, layers - 1));

			return Out.ToArray();
		}

		/// <summary>
		/// Returns all Files in the path within the selected <paramref name="layers"/>
		/// </summary>
		public static string[] GetFiles(string path, string pattern, int layers)
		{
			if (layers == 0)
				return new string[0];

			var Out = new List<string>();
			Out.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));

			foreach (var item in Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly))
				Out.AddRange(GetFiles(item, pattern, layers - 1));

			return Out.ToArray();
		}

		/// <summary>
		/// Gets all files in a directory that have any of the <paramref name="extensions"/> selected
		/// </summary>
		public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
		{
			if (extensions == null)
				throw new ArgumentNullException("extensions");
			IEnumerable<FileInfo> files = dir.EnumerateFiles("*.*", SearchOption.AllDirectories);
			return files.Where(f => extensions.Any(x => x.ToLower() == f.Extension.ToLower()));
		}

		/// <summary>
		/// Gets the Path of the Shortcut's Target
		/// </summary>
		public static string GetShortcutPath(this string Shortcut)
		{
			if (File.Exists(Shortcut))
			{
				// Add Reference > COM > Windows Script Host Object Model > OK
				IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell(); //Create a new WshShell Interface
				IWshRuntimeLibrary.IWshShortcut link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(Shortcut); //Link the interface to our shortcut

				return link.TargetPath; //Show the target in a MessagePrompt using IWshShortcut
			}
			return "";
		}

		/// <summary>
		/// Returns the <see cref="FileInfo"/> of the Target Path of a shortcut
		/// </summary>
		public static FileInfo GetShortcutPath(this FileInfo file)
			=> new FileInfo(GetShortcutPath(file.FullName));

		/// <summary>
		/// Checks if the <see cref="string"/> Path is a Folder
		/// </summary>
		/// <param name="CheckShortcuts">Checks if the Target of a shortcut is a Folder too</param>
		public static bool IsFolder(this string S, bool CheckShortcuts = true)
		{
			if (CheckShortcuts && S.EndsWith(".lnk"))
				S = S.GetShortcutPath();
			return (Directory.Exists(S)) && File.GetAttributes(S).HasFlag(FileAttributes.Directory);
		}

		public static bool IsFileLocked(this FileInfo file)
		{
			FileStream stream = null;

			try
			{
				stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
			}
			catch (IOException)
			{
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				return true;
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}

			//file is not locked
			return false;
		}

		/// <summary>
		/// Gets an <see cref="Array"/> containing the parents of a path
		/// </summary>
		/// <param name="depth">controls the amount of parents needed, keep null to return all parents</param>
		public static List<string> Parents(this string path, bool fullpath = false, int? depth = null)
		{
			if (!Directory.Exists(path) && !File.Exists(path))
				return new List<string>();

			var Out = new List<string>();
			var P = new DirectoryInfo(path);

			while ((P = P.Parent) != null && (depth == null || depth != Out.Count))
				Out.Add(fullpath ? P.FullName : P.Name);

			return Out;
		}

		#endregion I/O Extensions

		#region Number-Related

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

		#endregion Number-Related

		#region Time-Related

		public enum DateFormat { DMY, MDY, TDMY }

		/// <summary>
		/// Converts the <see cref="TimeSpan"/> into a readable large <see cref="string"/>
		/// </summary>
		/// <returns>x years?, x months?, x days?, x hours? : 'Today'</returns>
		public static string ToReadableBigString(this TimeSpan TS)
		{
			try
			{
				var hours = TS.Hours;
				var days = TS.Days;
				var years = days / 365; days -= years * 365;
				var months = days / 30; days -= months * 30;
				if (TS.TotalDays >= 1) days += hours > 0 ? 1 : 0;
				if (days > 30) { days -= 30; months++; }
				var Out = "";
				if (years > 0)
					Out = (years > 0 ? years + " year" + (years != 1 ? "s" : "") + ", " : "") + (months > 0 ? months + " month" + (months != 1 ? "s" : "") + ", " : "");
				else if (TS.TotalDays >= 1)
					Out = (months > 0 ? months + " month" + (months != 1 ? "s" : "") + ", " : "") + (days > 0 ? days + " day" + (days != 1 ? "s" : "") + ", " : "");
				else
					Out = (days > 0 ? days + " day" + (days != 1 ? "s" : "") + ", " : "") + (hours > 0 ? hours + " hour" + (hours != 1 ? "s" : "") + ", " : "");
				return Out.Substring(0, Out.Length - 2);
			}
			catch (Exception)
			{ return "Today"; }
		}

		/// <summary>
		/// Converts the <see cref="DateTime"/> to a readable long date
		/// </summary>
		/// <param name="AddYear">Adds the Year in the output</param>
		/// <returns>the 'xx'th of 'month' 'year'</returns>
		public static string ToReadableString(this DateTime DT, bool AddYear = true, DateFormat format = DateFormat.DMY)
		{
			if (DT == null)
				return null;

			var day = "";
			var month = "";
			if (DT.Day > 10 && DT.Day < 20)
				day = "th";
			else
				switch (DT.Day.ToString().Last())
				{
					case '1': day = "st"; break;
					case '2': day = "nd"; break;
					case '3': day = "rd"; break;
					default: day = "th"; break;
				}

			switch (DT.Month)
			{
				case 1: month = "January"; break;
				case 2: month = "February"; break;
				case 3: month = "March"; break;
				case 4: month = "April"; break;
				case 5: month = "May"; break;
				case 6: month = "June"; break;
				case 7: month = "July"; break;
				case 8: month = "August"; break;
				case 9: month = "September"; break;
				case 10: month = "October"; break;
				case 11: month = "November"; break;
				case 12: month = "December"; break;
				default: month = ""; break;
			}

			switch (format)
			{
				case DateFormat.DMY:
					return $"{DT.Day}{day} of {month}{(AddYear ? $" {DT.Year}" : "")}";

				case DateFormat.MDY:
					return $"{month} {DT.Day}{day}{(AddYear ? $" of {DT.Year}" : "")}";

				case DateFormat.TDMY:
					return $"the {DT.Day}{day} of {month}{(AddYear ? $" {DT.Year}" : "")}";

				default:
					return null;
			}
		}

		/// <summary>
		/// Converts the <see cref="TimeSpan"/> into a readable <see cref="string"/>
		/// </summary>
		/// <returns>x years?, x months?, x days?, x hours?, x minutes?, x seconds? : '0 seconds'</returns>
		public static string ToReadableString(this TimeSpan TS)
		{
			try
			{
				var days = TS.Days;
				var years = days / 365; days -= years * 365;
				var months = days / 30; days -= months * 30;
				var large = years > 0 || months > 1;
				var Out =
					((large ? (years > 0 ? years + " year" + (years != 1 ? "s" : "") + ", " : "") +
					(months > 0 ? months + " month" + (months != 1 ? "s" : "") + ", " : "") +
					(days > 0 ? days + " day" + (days != 1 ? "s" : "") + ", " : "") :
					(TS.Days > 0 ? TS.Days + " day" + (TS.Days != 1 ? "s" : "") + ", " : "")) +
					(TS.Hours > 0 ? TS.Hours + " hour" + (TS.Hours != 1 ? "s" : "") + ", " : "") +
					(TS.Minutes > 0 ? TS.Minutes + " minute" + (TS.Minutes != 1 ? "s" : "") + ", " : "") +
					(TS.Seconds > 0 ? TS.Seconds + " second" + (TS.Seconds != 1 ? "s" : "") + ", " : ""));
				return Out.Substring(0, Out.Length - 2);
			}
			catch (Exception)
			{ return "0 seconds"; }
		}

		#endregion Time-Related

		#region Threading

		/// <summary>
		/// Runs an <see cref="Action"/> in the background
		/// </summary>
		/// <param name="Priority">The <see cref="ThreadPriority"/> of the background <see cref="Thread"/></param>
		public static Thread RunInBackground(this Action A, ThreadPriority Priority = ThreadPriority.Normal)
		{
			var t = new Thread(new ThreadStart(A)) { IsBackground = true, Priority = Priority };
			t.Start();
			return t;
		}

		/// <summary>
		/// Runs an <see cref="Action"/> in the background after a delay
		/// </summary>
		/// <param name="Delay"><see cref="Action"/> delay in milliseconds</param>
		/// <param name="RunOnce">Option to run the <see cref="Action"/> once or repeating after each <paramref name="Delay"/></param>
		public static void RunInBackground(this Action A, int Delay, bool RunOnce = true)
		{
			var T = new System.Timers.Timer(Math.Max(1, Delay)) { AutoReset = !RunOnce };
			T.Elapsed += (s, e) => A();
			T.Start();
		}

		/// <summary>
		/// Loops an <see cref="Action"/> in the background until the <paramref name="condition"/> is met
		/// <param name="onEnd"><see cref="Action"/> to execute at the end</param>
		/// </summary>
		public static Thread TimerLoop(this Action action, Func<bool> condition, Action onEnd = null, ThreadPriority priority = ThreadPriority.Normal)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			if (condition == null)
				throw new ArgumentNullException(nameof(condition));

			var T = new Thread(() =>
			{
				while (condition())
					action();

				onEnd?.Invoke();
			})
			{
				IsBackground = true,
				Priority = priority
			};

			T.Start();

			return T;
		}

		#endregion Threading

		#region Others

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

		/// <summary>
		/// Returns <paramref name="changeValue"/> if this <see cref="{T}"/> is equal to the <paramref name="compareValue"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="compareValue">Value to compare with</param>
		/// <param name="changeValue">Value returned if the comparison is successful</param>
		public static T If<T>(this T baseValue, T compareValue, T changeValue)
			=> baseValue.Equals(compareValue) ? changeValue : baseValue;

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

		#endregion Others
	}
}