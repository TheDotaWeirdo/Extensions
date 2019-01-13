using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Extensions
{
	public enum FormDesignType { None, Dark, Light }

	public partial class FormDesign
	{
		public delegate void DesignEventHandler(FormDesign design);

		public static DesignEventHandler DesignChanged;

		private FormDesign(string name, int id, FormDesignType t)
		{
			Name = name; ID = id; Type = t;
		}

		#region Current Design

		private static FormDesign design = Modern;

		public static Image Loader
		{
			get
			{
				switch (Design.ID)
				{
					case 0: return Properties.Resources.Loader_0;
					case 1: return Properties.Resources.Loader_1;
					case 2: return Properties.Resources.Loader_2;
					case 3: return Properties.Resources.Loader_3;
					case 4: return Properties.Resources.Loader_4;
					case 5: return Properties.Resources.Loader_5;
					default: return Properties.Resources.Loader_0;
				}
			}
		}

		public static FormDesign Design
		{
			get => design;
			private set
			{
				if (value != design)
				{
					design = value;
					DesignChanged?.Invoke(value);
					if (!loadIdentifier.Disabled)
						Save();
				}
			}
		}

		#endregion Current Design

		#region Statics

		private static DisableIdentifier loadIdentifier = new DisableIdentifier();

		public static void Initialize(Form form, DesignEventHandler handler = null)
		{
			if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Shared")))
				Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Shared"));

			Load();

			if (handler != null)
			{
				DesignChanged += handler;
				handler(Design);
			}

			StartListener(form);
		}

		public static void Switch()
		{
			Design = List.Next(Design);
		}

		public static void Switch(FormDesign newDesign, bool forceSave = false, bool forceRefresh = false)
		{
			forceRefresh |= Design != newDesign;
			forceSave |= (forceRefresh && newDesign.Name != "Custom")/* || newDesign.Name == "Custom"*/;
			design = newDesign;

			if (forceRefresh)
				ForceRefresh();

			if (forceSave)
				Save();
		}

		public static bool IsCustomEligible()
			=> Custom.ID != -1 && Custom.BackColor.A != 0;

		public static void StartListener(Form form)
		{
			var watcher = new FileSystemWatcher
			{
				Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Shared"),
				NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
				Filter = "DesignMode.tf",
				EnableRaisingEvents = true
			};

			watcher.Changed += (s, e) =>
			{
				if (!loadIdentifier.Disabled)
					form.Invoke(new Action(Load));
			};
		}

		public static void Load()
		{
			loadIdentifier.Disable();
			try
			{
				var obj = ISave.LoadRaw("DesignMode.tf", "Shared");

				if (obj != null)
				{
					Custom = new FormDesign((string)obj.Custom.Name, (int)obj.Custom.ID, (FormDesignType)obj.Custom.Type)
					{
						BackColor = obj.Custom.BackColor,
						ButtonForeColor = obj.Custom.ButtonForeColor,
						MenuForeColor = obj.Custom.MenuForeColor,
						ForeColor = obj.Custom.ForeColor,
						ButtonColor = obj.Custom.ButtonColor,
						AccentColor = obj.Custom.AccentColor,
						MenuColor = obj.Custom.MenuColor,
						LabelColor = obj.Custom.LabelColor,
						InfoColor = obj.Custom.InfoColor,
						ActiveColor = obj.Custom.ActiveColor,
						ActiveForeColor = obj.Custom.ActiveForeColor,
						RedColor = obj.Custom.RedColor,
						GreenColor = obj.Custom.GreenColor,
						YellowColor = obj.Custom.YellowColor,
						IconColor = obj.Custom.IconColor
					};

					Design = List[(string)obj.Design];
				}
			}
			catch { }
			loadIdentifier.Enable();
		}

		public static void Save()
		{
			loadIdentifier.Disable();
			ISave.Save(new { Design = Design.ToString(), Custom }, "DesignMode.tf", appName: "Shared");
			loadIdentifier.Enable();
		}

		public static void ResetCustomTheme() => Custom = new FormDesign("Custom", -1, FormDesignType.None);

		public static void SetCustomBaseDesign(FormDesign design)
		{
			if (IsCustomEligible())
			{
				Custom.Type = design.Type;
				Custom.ID = design.ID;
			}
			else
			{
				Custom = new FormDesign("Custom", design.ID, design.Type)
				{
					BackColor = design.BackColor,
					ForeColor = design.ForeColor,
					ButtonColor = design.ButtonColor,
					ButtonForeColor = design.ButtonForeColor,
					AccentColor = design.AccentColor,
					MenuColor = design.MenuColor,
					MenuForeColor = design.MenuForeColor,
					LabelColor = design.LabelColor,
					InfoColor = design.InfoColor,
					ActiveColor = design.ActiveColor,
					ActiveForeColor = design.ActiveForeColor,
					RedColor = design.RedColor,
					GreenColor = design.GreenColor,
					YellowColor = design.YellowColor,
					IconColor = design.IconColor
				};
			}
		}

		public static void ForceRefresh() => DesignChanged?.Invoke(design);

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern bool SetSysColors(int cElements, int[] lpaElements, uint[] lpaRgbValues);

		private static void ChangeSelectColour()
		{
			const int COLOR_HIGHLIGHT = 13;
			const int COLOR_HIGHLIGHTTEXT = 14;
			// You will have to set the HighlightText colour if you want to change that as well.

			//array of elements to change
			int[] elements = { COLOR_HIGHLIGHT, COLOR_HIGHLIGHTTEXT };

			var colours = new List<uint>
			{
				(uint)ColorTranslator.ToWin32(FormDesign.Design.ActiveColor),
				(uint)ColorTranslator.ToWin32(FormDesign.Design.ActiveForeColor)
			};

			//set the desktop color using p/invoke
			SetSysColors(elements.Length, elements, colours.ToArray());
		}

		#endregion Statics

		#region Overrides

		public override bool Equals(object obj)
		{
			return obj is FormDesign design &&
					 ID == design.ID &&
					 Type == design.Type &&
					 Name == design.Name &&
					 EqualityComparer<Color>.Default.Equals(BackColor, design.BackColor) &&
					 EqualityComparer<Color>.Default.Equals(ForeColor, design.ForeColor) &&
					 EqualityComparer<Color>.Default.Equals(AccentColor, design.AccentColor) &&
					 EqualityComparer<Color>.Default.Equals(MenuColor, design.MenuColor) &&
					 EqualityComparer<Color>.Default.Equals(LabelColor, design.LabelColor) &&
					 EqualityComparer<Color>.Default.Equals(InfoColor, design.InfoColor) &&
					 EqualityComparer<Color>.Default.Equals(ActiveColor, design.ActiveColor) &&
					 EqualityComparer<Color>.Default.Equals(RedColor, design.RedColor) &&
					 EqualityComparer<Color>.Default.Equals(GreenColor, design.GreenColor) &&
					 EqualityComparer<Color>.Default.Equals(YellowColor, design.YellowColor) &&
					 EqualityComparer<Color>.Default.Equals(IconColor, design.IconColor);
		}

		public override int GetHashCode() => base.GetHashCode();

		public override string ToString() => Name;

		#endregion Overrides
	}
}