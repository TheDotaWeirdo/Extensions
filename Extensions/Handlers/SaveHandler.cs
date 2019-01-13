using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Extensions
{
	public static class SaveHandler
	{
		#region Single Save / File Methods
		[Obsolete("", true)]
		public static void Save(string path, Dictionary<string, object> data)
		{
			if (!File.Exists(path))
				Directory.CreateDirectory(Directory.GetParent(path).FullName);

			File.WriteAllLines(path, data.Convert(x => $"{x.Key.RegexRemove(":")}:{{{x.Value?.ToString() ?? string.Empty}}}".Replace("\r\n", "˩").Replace("\n", "˩")));
		}

		[Obsolete("", true)]
		public static void Save(string path, Dictionary<string, string> data)
		{
			if (!File.Exists(path))
				Directory.CreateDirectory(Directory.GetParent(path).FullName);

			File.WriteAllLines(path, data.Convert(x => $"{x.Key.RegexRemove(":")}:{{{x.Value}}}".Replace("\r\n", "˩").Replace("\n", "˩")));
		}

		[Obsolete]
		public static Dictionary<string, string> Load(string path)
			=> File.Exists(path) ? File.ReadAllLines(path).Convert(Convert).ToDictionary(x => x.Key, x => x.Value.Replace("˩", "\n")) : new Dictionary<string, string>();

		[Obsolete]
		private static KeyValuePair<string, string> Convert(string line)
		{
			var grps = Regex.Match(line, $"(.+?):{{(.+)?}}").Groups;
			return new KeyValuePair<string, string>(grps[1].Value, grps[2].Value);
		}
		#endregion

		#region Multiple Save /File Methods
		[Obsolete("", true)]
		public static void SaveMultiple(string path, IEnumerable<object> data)
		{
			ISave.Save(data, Path.GetFileName(path));
		}

		[Obsolete("", true)]
		public static void AddToMultiple(string path, params object[] data)
		{
			if (File.Exists(path))
				SaveMultiple(path, LoadMultiple(path).Concat(data.Convert(x => x.ToString().Replace("\r\n", "˩").Replace("\n", "˩"))));
			else
				SaveMultiple(path, data);
		}

		[Obsolete("", true)]
		public static void AddToMultipleDistinct(string path, params object[] data)
		{
			if (File.Exists(path))
				SaveMultiple(path, LoadMultiple(path).Concat(data.Convert(x => x.ToString())).Distinct());
			else
				SaveMultiple(path, data.Distinct());
		}

		[Obsolete]
		public static IEnumerable<string> LoadMultiple(string path)
		{
			try { return File.Exists(path) ? File.ReadAllLines(path).Convert(x => x.Replace("˩", "\n")) : new string[0]; }
			catch (IOException) { System.Threading.Thread.Sleep(5000); return LoadMultiple(path); }
		}

		#endregion

		#region App Settings

		[Obsolete]
		public static void SetSetting(string key, object value, ConfigurationUserLevel level = ConfigurationUserLevel.None)
		{
			var config = ConfigurationManager.OpenExeConfiguration(level);

			var entry = config.AppSettings.Settings[key];

			if (entry == null)
				config.AppSettings.Settings.Add(key, value.ToString());
			else
				config.AppSettings.Settings[key].Value = value.ToString();

			config.Save(ConfigurationSaveMode.Modified);
		}

		[Obsolete]
		public static T GetSetting<T>(string key, Func<string, T> func, T defaultValue, ConfigurationUserLevel level = ConfigurationUserLevel.None)
		{
			var config = ConfigurationManager.OpenExeConfiguration(level);

			try { return func(config.AppSettings.Settings[key].Value); }
			catch { return defaultValue; }
		}

		[Obsolete]
		public static string GetSetting(string key, ConfigurationUserLevel level = ConfigurationUserLevel.None)
		{
			var config = ConfigurationManager.OpenExeConfiguration(level);

			try { return config.AppSettings.Settings[key]?.Value ?? string.Empty; }
			catch { return string.Empty; }
		}

		[Obsolete]
		public static void RemoveSetting(string key, ConfigurationUserLevel level = ConfigurationUserLevel.None)
		{
			var config = ConfigurationManager.OpenExeConfiguration(level);

			if (config.AppSettings.Settings[key] != null)
				config.AppSettings.Settings.Remove(key);

			config.Save(ConfigurationSaveMode.Modified);
		}

		#endregion

		[Obsolete]
		public static T LoadOne<T>(this Dictionary<string, string> data, string key, Func<string, T> func, T defaultValue)
		{
			try { return data.ContainsKey(key) ? func(data[key]) : defaultValue; }
			catch { return defaultValue; }
		}

		[Obsolete]
		public static string LoadOne(this Dictionary<string, string> data, string key)
		{
			try { return data.ContainsKey(key) ? data[key] : string.Empty; }
			catch { return string.Empty; }
		}

		[Obsolete]
		public static string ForSave<T>(this IEnumerable<T> list, Func<T, string> func = null) 
			=> list.ListStrings(x => $"{{{(func == null ? x.ToString() : func(x))}}}");

		[Obsolete]
		public static List<T> FromSave<T>(this string s, Func<string, T> func)
			=> Regex.Matches(s, "{(.+?)}").Convert(x => func(x.Groups[1].Value)).ToList();
	}
}
