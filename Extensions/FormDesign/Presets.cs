﻿using System.ComponentModel;
using System.Drawing;

namespace Extensions
{
	partial class FormDesign
	{
		public static DesignList List
		{
			get
			{
				var list = new DesignList()
				{
					Modern,
					BerryBlues,
					Ice,
					Chic,
					Strawberries,
					Dark
				};

				if (IsCustomEligible())
					list.Add(Custom);

				return list;
			}
		}

		public static FormDesign Modern = new FormDesign("Modern", 0, FormDesignType.Light)
		{
			BackColor = Color.FromArgb(239, 243, 248),
			ForeColor = Color.FromArgb(50, 58, 69),
			ButtonColor = Color.FromArgb(219, 225, 234),
			ButtonForeColor = Color.FromArgb(72, 84, 100),
			AccentColor = Color.FromArgb(177, 192, 212),
			MenuColor = Color.FromArgb(51, 63, 79),
			MenuForeColor = Color.FromArgb(191, 202, 218),
			LabelColor = Color.FromArgb(99, 114, 135),
			InfoColor = Color.FromArgb(132, 146, 165),
			ActiveColor = Color.FromArgb(232, 157, 22),
			ActiveForeColor = Color.FromArgb(250, 250, 250),
			RedColor = Color.FromArgb(180, 59, 34),
			GreenColor = Color.FromArgb(170, 224, 113),
			YellowColor = Color.FromArgb(229, 193, 52),
			IconColor = Color.FromArgb(77, 87, 102)
		};

		public static FormDesign BerryBlues = new FormDesign("Berry Blues", 1, FormDesignType.Dark)
		{
			BackColor = Color.FromArgb(35, 37, 50),
			ForeColor = Color.FromArgb(227, 231, 235),
			ButtonColor = Color.FromArgb(42, 45, 68),
			ButtonForeColor = Color.FromArgb(199, 210, 221),
			AccentColor = Color.FromArgb(58, 77, 110),
			MenuColor = Color.FromArgb(30, 31, 38),
			MenuForeColor = Color.FromArgb(181, 203, 225),
			LabelColor = Color.FromArgb(101, 126, 170),
			InfoColor = Color.FromArgb(115, 137, 169),
			ActiveColor = Color.FromArgb(67, 107, 234),
			ActiveForeColor = Color.FromArgb(208, 225, 249),
			RedColor = Color.FromArgb(175, 58, 136),
			GreenColor = Color.FromArgb(65, 185, 172),
			YellowColor = Color.FromArgb(231, 233, 60),
			IconColor = Color.FromArgb(227, 231, 235)
		};

		public static FormDesign Ice = new FormDesign("Ice", 2, FormDesignType.Light)
		{
			BackColor = Color.FromArgb(241, 241, 242),
			ForeColor = Color.FromArgb(45, 69, 85),
			ButtonColor = Color.FromArgb(223, 229, 237),
			ButtonForeColor = Color.FromArgb(23, 118, 136),
			AccentColor = Color.FromArgb(206, 223, 227),
			MenuColor = Color.FromArgb(233, 236, 238),
			MenuForeColor = Color.FromArgb(88, 108, 121),
			LabelColor = Color.FromArgb(102, 143, 168),
			InfoColor = Color.FromArgb(132, 151, 172),
			ActiveColor = Color.FromArgb(25, 149, 173),
			ActiveForeColor = Color.FromArgb(241, 241, 242),
			RedColor = Color.FromArgb(248, 110, 120),
			GreenColor = Color.FromArgb(101, 223, 172),
			YellowColor = Color.FromArgb(250, 218, 134),
			IconColor = Color.FromArgb(83, 125, 151)
		};

		public static FormDesign Chic = new FormDesign("Chic", 3, FormDesignType.Light)
		{
			BackColor = Color.FromArgb(251, 248, 237),
			ButtonForeColor = Color.FromArgb(116, 106, 97),
			MenuForeColor = Color.FromArgb(116, 106, 97),
			ForeColor = Color.FromArgb(116, 106, 97),
			ButtonColor = Color.FromArgb(233, 217, 203),
			AccentColor = Color.FromArgb(227, 211, 195),
			MenuColor = Color.FromArgb(240, 235, 223),
			LabelColor = Color.FromArgb(157, 145, 133),
			InfoColor = Color.FromArgb(171, 161, 152),
			ActiveColor = Color.FromArgb(212, 161, 124),
			ActiveForeColor = Color.FromArgb(245, 239, 225),
			RedColor = Color.FromArgb(153, 57, 46),
			GreenColor = Color.FromArgb(175, 214, 104),
			YellowColor = Color.FromArgb(255, 198, 64),
			IconColor = Color.FromArgb(173, 147, 120)
		};

		public static FormDesign Strawberries = new FormDesign("Strawberries", 4, FormDesignType.Light)
		{
			BackColor = Color.FromArgb(249, 246, 242),
			ForeColor = Color.FromArgb(112, 55, 26),
			ButtonColor = Color.FromArgb(247, 225, 212),
			ButtonForeColor = Color.FromArgb(144, 72, 29),
			AccentColor = Color.FromArgb(235, 185, 166),
			MenuColor = Color.FromArgb(246, 238, 229),
			MenuForeColor = Color.FromArgb(160, 114, 104),
			LabelColor = Color.FromArgb(210, 149, 114),
			InfoColor = Color.FromArgb(219, 193, 185),
			ActiveColor = Color.FromArgb(215, 92, 77),
			ActiveForeColor = Color.FromArgb(252, 253, 254),
			RedColor = Color.FromArgb(224, 95, 68),
			GreenColor = Color.FromArgb(188, 224, 113),
			YellowColor = Color.FromArgb(224, 158, 67),
			IconColor = Color.FromArgb(210, 149, 114)
		};

		public static FormDesign Dark = new FormDesign("Dark", 5, FormDesignType.Dark)
		{
			BackColor = Color.FromArgb(18, 20, 23),
			ForeColor = Color.FromArgb(215, 218, 224),
			ButtonColor = Color.FromArgb(33, 37, 43),
			ButtonForeColor = Color.FromArgb(191, 195, 201),
			AccentColor = Color.FromArgb(69, 75, 86),
			MenuColor = Color.FromArgb(22, 24, 29),
			MenuForeColor = Color.FromArgb(157, 165, 180),
			LabelColor = Color.FromArgb(171, 178, 191),
			InfoColor = Color.FromArgb(92, 99, 112),
			ActiveColor = Color.FromArgb(44, 199, 197),
			ActiveForeColor = Color.FromArgb(33, 43, 45),
			RedColor = Color.FromArgb(222, 74, 85),
			GreenColor = Color.FromArgb(86, 203, 152),
			YellowColor = Color.FromArgb(205, 151, 100),
			IconColor = Color.FromArgb(175, 178, 183)
		};

		public static FormDesign Custom = new FormDesign("Custom", -1, FormDesignType.None);
	}
}