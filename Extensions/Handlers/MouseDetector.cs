using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace Extensions
{
	public class MouseDetector : IDisposable
	{
		#region APIs

		[DllImport("gdi32")]
		public static extern uint GetPixel(IntPtr hDC, int XPos, int YPos);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool GetCursorPos(out POINT pt);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		#endregion

		Timer tm = new Timer() { Interval = 10 };
		public delegate void MouseMoveDLG(object sender, Point p);
		public event MouseMoveDLG MouseMove;
		public MouseDetector()
		{
			tm.Tick += new EventHandler(Tm_Tick); tm.Start();
		}

		void Tm_Tick(object sender, EventArgs e)
		{
			GetCursorPos(out var p);
			MouseMove?.Invoke(this, new Point(p.X, p.Y));
		}

		public void Dispose()
		{
			tm.Dispose();
			MouseMove = null;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;
			public POINT(int x, int y)
			{
				X = x;
				Y = y;
			}
		}
	}
}
