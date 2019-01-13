namespace Extensions
{
	public class DisableIdentifier
	{
		private static ulong IdList = 0;
		private bool disabled = false;

		public DisableIdentifier() => ID = IdList++;

		public DisableIdentifier(bool enabled)
		{
			disabled = !enabled; ID = IdList++;
		}

		public bool Disabled => disabled;
		public bool Enabled => !disabled;
		public ulong ID { get; protected set; }

		public virtual void Disable() => disabled = true;

		public bool Disable(int milliseconds)
		{
			if (disabled)
				return true;
			disabled = true;
			var T = new System.Timers.Timer(milliseconds) { AutoReset = false };
			T.Elapsed += (s, e) => disabled = false;
			T.Start();
			return false;
		}

		public virtual void Enable() => disabled = false;
	}
}