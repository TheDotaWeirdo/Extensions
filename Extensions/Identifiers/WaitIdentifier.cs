namespace Extensions
{
	public class WaitIdentifier : DisableIdentifier
	{
		private static ulong IdList = 0;

		private System.Timers.Timer timer;

		private bool waiting = false;

		public WaitIdentifier() => ID = IdList++;

		public delegate void MyAction();

		public bool Waiting => waiting;

		public void Refresh()
		{
			timer.Stop(); timer.Start();
		}

		public override void Disable()
		{
			timer?.Dispose();
			base.Disable();
		}

		public void Wait(MyAction action, int milliseconds)
		{
			try
			{
				waiting = true;
				timer?.Dispose();
				timer = new System.Timers.Timer(milliseconds) { AutoReset = false };
				timer.Elapsed += (s, e) => { action(); timer.Dispose(); waiting = false; };
				timer?.Start();
			}
			catch { }
		}
	}
}