using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Extensions
{
	public class Promise
	{
		public delegate void ThenAction();
		public delegate void ErrorAction(Exception error);

		private Exception GetException;

		public Promise() { }

		public Promise(Exception exception) => GetException = exception;

		public void Then(ThenAction thenAction, ErrorAction errorAction)
		{
			if (GetException != null)
				errorAction(GetException);
			else
			{
				try { thenAction(); }
				catch (Exception ex) { errorAction(ex); }
			}
		}

		public void Then(ThenAction thenAction)
		{
			if (GetException == null)
			{
				try { thenAction(); }
				catch (Exception) { }
			}
		}
	}

	public class Promise<T>
	{
		public delegate void ThenAction(T result);
		public delegate void ErrorAction(Exception error);

		public T Result { get; private set; }
		private Exception GetException;

		public Promise(T result) => Result = result;

		public Promise(Exception exception) => GetException = exception;

		public void Then(ThenAction thenAction, ErrorAction errorAction)
		{
			if (GetException != null)
				errorAction(GetException);
			else
			{
				try { thenAction(Result); }
				catch (Exception ex) { errorAction(ex); }
			}
		}

		public void Then(ThenAction thenAction)
		{
			if (GetException == null)
			{
				try { thenAction(Result); }
				catch (Exception) { }
			}
		}
	}

	public class Promise<T1, T2>
	{
		public delegate void ThenAction(T1 result1, T2 result2);
		public delegate void ErrorAction(Exception error);

		public T1 Result1 { get; private set; }
		public T2 Result2 { get; private set; }
		private Exception GetException;

		public Promise(T1 result1, T2 result2)
		{
			Result1 = result1;
			Result2 = result2;
		}

		public Promise(Exception exception) => GetException = exception;

		public void Then(ThenAction thenAction, ErrorAction errorAction)
		{
			if (GetException != null)
				errorAction(GetException);
			else
			{
				try { thenAction(Result1, Result2); }
				catch (Exception ex) { errorAction(ex); }
			}
		}

		public void Then(ThenAction thenAction)
		{
			if (GetException == null)
			{
				try { thenAction(Result1, Result2); }
				catch (Exception) { }
			}
		}
	}
}
