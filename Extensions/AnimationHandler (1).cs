using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Extensions;

namespace Extensions
{
	public class AnimationHandler : IDisposable
	{
		public Control AnimatedControl { get; private set; }
		public Size OriginalSize { get; private set; }
		public Size NewSize { get; private set; }

		public int Interval { get; set; }
		public double SpeedModifier { get; set; }

		public bool IgnoreWidth { get; set; }
		public bool IgnoreHeight { get; set; }
		public bool Lazy { get; set; }

		private System.Timers.Timer Timer;
		private Action endAction;

		public AnimationHandler(Control animatedControl, Size newSize)
		{
			AnimatedControl = animatedControl;
			OriginalSize = animatedControl.Size;
			NewSize = newSize;

			Interval = 16;
			SpeedModifier = 6;
		}

		public void StartAnimation(Action action = null)
		{
			endAction = action;
			Timer = new System.Timers.Timer(Interval);

			Timer.Elapsed += Timer_Tick;

			Timer.Start();
		}

		public void StopAnimation() => Timer?.Dispose();

		public void ResumeAnimation() => Timer?.Start();

		public void PauseAnimation() => Timer?.Stop();

		private void Timer_Tick(object sender, ElapsedEventArgs e)
		{
			if (AnimatedControl == null)
			{
				Timer?.Dispose();
				return;
			}

			try
			{
				var incWidth = ( NewSize.Width - AnimatedControl.Width ).Sign() == 1;
				var nextWidth = (int)( AnimatedControl.Width + ( NewSize.Width - AnimatedControl.Width ) / SpeedModifier );

				if (incWidth)
					nextWidth = nextWidth.Between(AnimatedControl.Width + 1, NewSize.Width);
				else
					nextWidth = nextWidth.Between(NewSize.Width, AnimatedControl.Width - 1);

				var incHeight = ( NewSize.Height - AnimatedControl.Height ).Sign() == 1;
				var nextHeight = (int)( AnimatedControl.Height + ( NewSize.Height - AnimatedControl.Height ) / SpeedModifier );

				if (incHeight)
					nextHeight = nextHeight.Between(AnimatedControl.Height + 1, NewSize.Height);
				else
					nextHeight = nextHeight.Between(NewSize.Height, AnimatedControl.Height - 1);

				try
				{
					if (AnimatedControl.InvokeRequired)
						AnimatedControl.Invoke(new Action(() => { try { AnimatedControl.Size = new Size(nextWidth, nextHeight); } catch { } }));
					else
						AnimatedControl.Size = new Size(nextWidth, nextHeight);
				}
				catch { }

				if (!disposedValue && CheckIfEnded())
				{
					if (AnimatedControl?.InvokeRequired ?? true)
						AnimatedControl?.Invoke(new Action(() =>
						{
							try
							{
								AnimatedControl.Size = NewSize;
								endAction?.Invoke();
							}
							catch { }
						}));
					else
					{
						try
						{
							AnimatedControl.Size = NewSize;
							endAction?.Invoke();
						}
						catch { }
					}

					Dispose();
				}
			}
			catch { }
		}

		private bool CheckIfEnded()
		{
			if (!IgnoreWidth)
			{
				if (Lazy)
				{
					if (Math.Abs(AnimatedControl.Width - NewSize.Width) > 5)
						return false;
				}
				else if (AnimatedControl.Width != NewSize.Width)
					return false;
			}

			if (!IgnoreHeight)
			{
				if (Lazy)
				{
					if (Math.Abs(AnimatedControl.Height - NewSize.Height) > 5)
						return false;
				}
				else if (AnimatedControl.Height != NewSize.Height)
					return false;
			}

			return true;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Timer?.Dispose();
				}

				AnimatedControl = null;

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}
