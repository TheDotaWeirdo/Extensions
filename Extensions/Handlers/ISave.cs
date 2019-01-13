using System;
using System.IO;
using Newtonsoft.Json;
using static System.Environment;

namespace Extensions
{
	public class ISave
	{
		public static string AppName { get; set; }

		public static string DocsFolder => Path.Combine(GetFolderPath(SpecialFolder.MyDocuments), AppName);

		public virtual string Name { get; set; }

		public virtual void OnLoad() { }

		#region Load

		public static T Load<T>(string name, string appName = null) where T : ISave, new()
		{
			var doc = GetPath(name, appName);
			var settings = new JsonSerializerSettings();
			settings.Error += (o, args) =>
			{
				args.ErrorContext.Handled = true;
			};

			if (File.Exists(doc))
			{
				var res = JsonConvert.DeserializeObject<T>(File.ReadAllText(doc), settings);
				res.OnLoad();
				return res;
			}

			return new T() { Name = name };
		}

		public static void Load<T>(out T obj, string name, string appName = null) where T : new()
		{
			var doc = GetPath(name, appName);

			if (File.Exists(doc) && new FileInfo(doc).Length > 0)
				obj = JsonConvert.DeserializeObject<T>(File.ReadAllText(doc));
			else
				obj = new T();
		}

		public static dynamic LoadRaw(string name, string appName = null)
		{
			var doc = GetPath(name, appName);

			if (File.Exists(doc))
				return JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(doc));
			else
				return null;
		}

		#endregion Load

		#region Save

		public void Save(string name = null, bool supressErrors = false, string appName = null)
		{
			if (string.IsNullOrWhiteSpace(name.IfEmpty(Name)))
				throw new MissingFieldException("ISave", "Name");

			var doc = GetPath(name.IfEmpty(Name), appName);
			var settings = new JsonSerializerSettings();
			settings.Error += (o, args) =>
			{
				if (supressErrors)
					args.ErrorContext.Handled = true;
			};

			Write(doc, JsonConvert.SerializeObject(this, Formatting.Indented, settings));
		}

		public static void Save(object obj, string name, bool supressErrors = false, string appName = null)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new MissingFieldException("ISave", "Name");

			var doc = GetPath(name, appName);
			var settings = new JsonSerializerSettings();
			settings.Error += (o, args) =>
			{
				if (supressErrors)
					args.ErrorContext.Handled = true;
			};

			Write(doc, JsonConvert.SerializeObject(obj, Formatting.Indented, settings));
		}

		#endregion Save

		#region Other

		public void Delete(string name = null, string appName = null)
		{
			if (string.IsNullOrWhiteSpace(name.IfEmpty(Name)))
				throw new MissingFieldException("ISave", "Name");

			var doc = GetPath(name.IfEmpty(Name), appName);

			File.Delete(doc);
		}

		private static void Write(string path, string content)
		{
			var tries = 0;
			Directory.GetParent(path).Create();
			retry: tries++;
			try { File.WriteAllText(path, content); }
			catch (Exception ex)
			{
				if (tries < 5)
				{
					System.Threading.Thread.Sleep(1500);
					goto retry;
				}
				throw ex;
			}
		}

		private static string GetPath(string name, string appName = null)
			=> Path.Combine(GetFolderPath(SpecialFolder.MyDocuments), appName.IfEmpty(AppName), name);

		#endregion Other
	}
}