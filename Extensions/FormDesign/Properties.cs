using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Extensions
{
	public partial class FormDesign
	{
		public int ID { get; private set; }
		public string Name { get; private set; }
		public FormDesignType Type { get; private set; }

		public Color BackColor { get; set; }
		public Color ForeColor { get; set; }
		public Color ButtonColor { get; set; }
		public Color ButtonForeColor { get; set; }
		public Color AccentColor { get; set; }
		public Color MenuColor { get; set; }
		public Color MenuForeColor { get; set; }
		public Color LabelColor { get; set; }
		public Color InfoColor { get; set; }
		public Color ActiveColor { get; set; }
		public Color ActiveForeColor { get; set; }
		public Color RedColor { get; set; }
		public Color GreenColor { get; set; }
		public Color YellowColor { get; set; }
		public Color IconColor { get; set; }

		[JsonIgnore]
		public Color AccentBackColor => BackColor.Tint(Lum: Type.If(FormDesignType.Dark, 3, -3));
	}
}
