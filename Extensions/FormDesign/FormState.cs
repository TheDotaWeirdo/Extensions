using System.Drawing;

namespace Extensions
{
	public enum FormState
	{
		NormalUnfocused	= 0,
		NormalFocused		= 1,
		Busy					= 2,
		Working				= 3,
		Running				= 4,
		ForcedFocused		= 5
	}

	public static class FormStateExt
	{
		public static bool IsNormal(this FormState state) => state == FormState.NormalUnfocused || state == FormState.NormalFocused;

		public static FormState Normal(bool active) => active ? FormState.NormalFocused : FormState.NormalUnfocused;
		
		public static Color Color(this FormState state) 
		{
			switch (state)
			{
				case FormState.NormalUnfocused:
					return FormDesign.Design.MenuColor;

				case FormState.NormalFocused:
					return FormDesign.Design.ActiveColor;

				case FormState.ForcedFocused:
					return FormDesign.Design.ActiveColor;

				case FormState.Busy:
					return FormDesign.Design.RedColor;

				case FormState.Working:
					return FormDesign.Design.YellowColor;

				case FormState.Running:
					return FormDesign.Design.GreenColor;

				default:
					return FormDesign.Design.MenuColor;
			}
		} 
	}
}
