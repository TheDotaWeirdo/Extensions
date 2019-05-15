using System;
using System.Linq;

namespace Extensions
{
	public static partial class ExtensionClass
	{
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

		public static string ToRelatedString(this DateTime dt)
		{
			var ts = new TimeSpan(Math.Abs(dt.Ticks - DateTime.Now.Ticks));
			var past = dt < DateTime.Now;

			if (ts.TotalHours < 5)
			{
				if (past)
					return ts.ToReadableString() + " ago";
				return "in " + ts.ToReadableString();
			}
			else if (dt.Date == DateTime.Today)
				return $"Today at {dt.ToString("h:mm tt")}";
			else if (dt.Date.AnyOf(DateTime.Today.AddDays(1), DateTime.Today.AddDays(-1)))
				return (past ? "Yesterday at " : "Tomorrow at ") + dt.ToString("h:mm tt");
			else if (ts.TotalDays < 7)
				return $"{(past ? "Last " : "Next ")}{dt.DayOfWeek.ToString()} at {dt.ToString("h:mm tt")}";

			var days = ts.Days;
			var years = days / 365;
			days -= years * 365;
			var months = days / 30;

			if (years > 0)
				return (past ? $"{years} years ago" : $"in {years} years") + $" on {dt.ToReadableString(true, DateFormat.TDMY)}";

			if (months > 0)
				return (past ? $"{months} months ago" : $"in {months} months") + $" on {dt.ToReadableString(false, DateFormat.TDMY)}";

			return (past ? $"{days} days ago" : $"in {days} days") + $" on {dt.ToReadableString(false, DateFormat.TDMY)}";
		}
	}
}