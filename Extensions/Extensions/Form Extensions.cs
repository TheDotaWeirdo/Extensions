using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace Extensions
{
	public static partial class ExtensionClass
	{
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

		public static Point Center(this Rectangle rect, Size controlSize)
			=> new Point((rect.Width - controlSize.Width) / 2 + rect.X, (rect.Height - controlSize.Height) / 2 + rect.Y);

		public static Point Center(this Size rect, Size controlSize)
			=> new Point((rect.Width - controlSize.Width) / 2, (rect.Height - controlSize.Height) / 2);

		public static PointF Center(this RectangleF rect, SizeF controlSize)
			=> new PointF((rect.Width - controlSize.Width) / 2 + rect.X, (rect.Height - controlSize.Height) / 2 + rect.Y);

		public static PointF Center(this SizeF rect, SizeF controlSize)
			=> new PointF((rect.Width - controlSize.Width) / 2, (rect.Height - controlSize.Height) / 2);

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

		public static IEnumerable<T> GetControls<T>(this Form form) where T : Control
		 => GetControls<T>(form);

		public static IEnumerable<T> GetControls<T>(this Control control) where T : Control
		{
			if (control is T)
				yield return (control as T);

			foreach (Control item in control.Controls)
				foreach (var ctrl in GetControls<T>(item))
					yield return (ctrl as T);
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
		/// Checks if an <see cref="Image"/> is an animated GIF
		/// </summary>
		public static bool IsAnimated(this Image img)
			=> img != null && img.GetFrameCount(new FrameDimension(img.FrameDimensionsList[0])) > 1;

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
			if (panel == null || panel.Controls == null)
				return;

			var controls = panel.Controls.Cast<Control>();

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

			var controls = panel.Controls.Cast<Control>();

			var index = 0;
			if (suspendDrawing)
				panel.SuspendDrawing();
			foreach (var item in controls.OrderByDescending(keySelector))
				panel.Controls.SetChildIndex(item, index++);
			if (suspendDrawing)
				panel.ResumeDrawing();
		}

		/// <summary>
		/// Sorts the controls of a panel using a <paramref name="keySelector"/>
		/// </summary>
		public static void OrderBy<TKey>(this Control.ControlCollection ctrls, Func<Control, TKey> keySelector, bool suspendDrawing = true)
		{
			if (ctrls == null)
				return;

			var controls = ctrls.Cast<Control>();

			if (controls.Count() > 0 && !controls.SequenceEqual(controls.OrderByDescending(keySelector)))
			{
				var panel = controls.First().Parent;

				var index = 0;
				if (suspendDrawing)
					panel.SuspendDrawing();
				foreach (var item in controls.OrderBy(keySelector))
					panel.Controls.SetChildIndex(item, index++);
				if (suspendDrawing)
					panel.ResumeDrawing();
			}
		}

		/// <summary>
		/// Sorts the controls of a panel using a <paramref name="keySelector"/> in a descending order
		/// </summary>
		public static void OrderByDescending<TKey>(this Control.ControlCollection ctrls, Func<Control, TKey> keySelector, bool suspendDrawing = true)
		{
			if (ctrls == null)
				return;

			var controls = ctrls.Cast<Control>();

			if (controls.Count() > 0 && !controls.SequenceEqual(controls.OrderByDescending(keySelector)))
			{
				var panel = controls.First().Parent;

				var index = 0;
				if (suspendDrawing)
					panel.SuspendDrawing();
				foreach (var item in controls.OrderByDescending(keySelector))
					panel.Controls.SetChildIndex(item, index++);
				if (suspendDrawing)
					panel.ResumeDrawing();
			}
		}

		public static void DrawStringItem(this Graphics graphics, object item, Font font, Color foreColor, int maxWidth, double tab, ref int height, bool draw = true)
		{
			var x = (int)(tab * 12 + 5);
			var bnds = graphics.MeasureString(item.ToString(), font, maxWidth - x);

			if (draw)
				graphics.DrawString(item.ToString(), font, new SolidBrush(foreColor), new Rectangle(x, height, maxWidth - x, (int)Math.Ceiling(bnds.Height)));

			height += (int)(bnds.Height * 1.1F);
		}

		public static void RecursiveClick(this Control control, EventHandler handler)
		{
			control.Click += handler;

			foreach (Control child in control.Controls)
				RecursiveClick(child, handler);
		}

		public static Color GetAverageColor(this Image bmp, Rectangle? rectangle = null)
			=> (bmp as Bitmap).GetAverageColor(rectangle);

		public static Color GetAverageColor(this Bitmap bmp, Rectangle? rectangle = null)
		{
			var rect = rectangle ?? new Rectangle(Point.Empty, bmp.Size);
			var r = 0;
			var g = 0;
			var b = 0;

			//if (rectangle != null)
			//	rect = new Rectangle(Math.Max(0, rect.X), Math.Max(0, rect.Y), Math.Max(bmp.Width, rect.Width), Math.Max(bmp.Height, rect.Height));

			var total = 0;

			for (var x = rect.X; x <= rect.X + rect.Width; x++)
			{
				for (var y = rect.Y; y < rect.Y + rect.Height; y++)
				{
					var clr = bmp.GetPixel(x, y);

					r += clr.R;
					g += clr.G;
					b += clr.B;

					total++;
				}
			}

			//Calculate average
			r /= total;
			g /= total;
			b /= total;

			return System.Drawing.Color.FromArgb(r, g, b);
		}

		public static Color GetTextColor(this Color color)
		{
			var b = color.GetBrightness();

			if (b > .75)
				return ColorFromHSL(color.GetHue(), color.GetSaturation(), .2);

			if (b < .25)
				return ColorFromHSL(color.GetHue(), color.GetSaturation(), .8);

			if (b > .5)
				return ColorFromHSL(color.GetHue(), color.GetSaturation(), .05);

			return ColorFromHSL(color.GetHue(), color.GetSaturation(), .95);
		}

		/// <summary>
		/// Resumes the Drawing of a <see cref="Control"/>
		/// </summary>
		public static void ResumeDrawing(this Control parent, bool refresh = true)
		{
			try
			{
				SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
				if (refresh)
				{
					parent.Refresh();
					parent.ResumeLayout(true);
				}
			}
			catch { }
		}

		public static GraphicsPath RoundedRect(this Rectangle bounds, int radius, bool topleft = true, bool topright = true, bool botright = true, bool botleft = true)
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
			if (topleft)
				path.AddArc(arc, 180, 90);
			else
				path.AddLine(bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y);

			// top right arc
			arc.X = bounds.Right - diameter;
			if (topright)
				path.AddArc(arc, 270, 90);
			else
				path.AddLine(bounds.X + bounds.Width, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height);

			// bottom right arc
			arc.Y = bounds.Bottom - diameter;
			if (botright)
				path.AddArc(arc, 0, 90);
			else
				path.AddLine(bounds.X + bounds.Width, bounds.Y + bounds.Height, bounds.X, bounds.Y + bounds.Height);

			// bottom left arc
			arc.X = bounds.Left;
			if (botleft)
				path.AddArc(arc, 90, 90);
			else
				path.AddLine(bounds.X, bounds.Y + bounds.Height, bounds.X, bounds.Y);

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
			form.Focus();
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
	}
}