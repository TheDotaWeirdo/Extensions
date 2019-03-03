using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Extensions
{
	public static partial class ExtensionClass
	{
		public static string AbreviatedPath(this FileInfo file, bool folder = false)
			=> AbreviatedPath(folder ? file.DirectoryName : file.FullName);

		public static string AbreviatedPath(this DirectoryInfo folder)
			=> AbreviatedPath(folder.FullName);

		private static string AbreviatedPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				return string.Empty;

			var items = path.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
			var selectedItems = new List<string>() { items[0] };

			selectedItems.AddRange(items.TakeLast(Math.Min(3, items.Length - 1)));

			if (items.Length != selectedItems.Count)
				selectedItems.Insert(1, "..");

			return selectedItems.ListStrings("\\");
		}

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
		public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, SearchOption searchOption, params string[] extensions)
		{
			if (extensions == null)
				throw new ArgumentNullException("extensions");
			IEnumerable<FileInfo> files = dir.EnumerateFiles("*.*", searchOption);
			return files.Where(f => extensions.Any(x => x.ToLower() == f.Extension.ToLower()));
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
	}
}