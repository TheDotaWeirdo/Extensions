using System.Threading;

namespace Extensions
{
	public class OneWayTask
	{
		private Thread currentTask;

		public delegate void MyAction();

		public void Run(MyAction action, bool isBackground = true, ThreadPriority threadPriority = ThreadPriority.Normal)
		{
			if (currentTask != null && currentTask.IsAlive)
				currentTask.Abort();

			currentTask = new Thread(new ThreadStart(action))
			{
				IsBackground = isBackground,
				Priority = threadPriority
			};
			try { currentTask.Start(); }
			catch { }
		}
	}
}