using System;
using System.Threading;

namespace Extensions
{
	public static partial class ExtensionClass
	{
		/// <summary>
		/// Runs an <see cref="Action"/> in the background
		/// </summary>
		/// <param name="Priority">The <see cref="ThreadPriority"/> of the background <see cref="Thread"/></param>
		public static Thread RunInBackground(this Action A, ThreadPriority Priority = ThreadPriority.Normal)
		{
			var t = new Thread(new ThreadStart(A)) { IsBackground = true, Priority = Priority };
			t.Start();
			return t;
		}

		/// <summary>
		/// Runs an <see cref="Action"/> in the background after a delay
		/// </summary>
		/// <param name="Delay"><see cref="Action"/> delay in milliseconds</param>
		/// <param name="RunOnce">Option to run the <see cref="Action"/> once or repeating after each <paramref name="Delay"/></param>
		public static void RunInBackground(this Action A, int Delay, bool RunOnce = true)
		{
			var T = new System.Timers.Timer(Math.Max(1, Delay)) { AutoReset = !RunOnce };
			T.Elapsed += (s, e) => A();
			T.Start();
		}

		/// <summary>
		/// Loops an <see cref="Action"/> in the background until the <paramref name="condition"/> is met
		/// <param name="onEnd"><see cref="Action"/> to execute at the end</param>
		/// </summary>
		public static Thread TimerLoop(this Action action, Func<bool> condition, Action onEnd = null, ThreadPriority priority = ThreadPriority.Normal)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			if (condition == null)
				throw new ArgumentNullException(nameof(condition));

			var T = new Thread(() =>
			{
				while (condition())
					action();

				onEnd?.Invoke();
			})
			{
				IsBackground = true,
				Priority = priority
			};

			T.Start();

			return T;
		}
	}
}