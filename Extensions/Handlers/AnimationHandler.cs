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
		public Control AnimatedControl { get; }
		public Size OriginalSize { get; }
		public Size NewSize { get; set; }

		public int Interval { get; set; }
		public double SpeedModifier { get; set; }

		public bool IgnoreWidth { get; set; }
		public bool IgnoreHeight { get; set; }
		public bool Lazy { get; set; }

		private System.Timers.Timer Timer;
		private Action endAction;

		public delegate void AnimationTick(AnimationHandler handler, Control control, double percentage);

		public event AnimationTick OnAnimationTick;
		public event AnimationTick OnEnd;

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
				var incWidth = (NewSize.Width - AnimatedControl.Width).Sign() == 1;
				var nextWidth = (int)(AnimatedControl.Width + ((NewSize.Width - AnimatedControl.Width) / SpeedModifier).Between(incWidth.If(3, -50), incWidth.If(50, -3)));

				if (incWidth)
					nextWidth = nextWidth.Between(AnimatedControl.Width + 1, NewSize.Width);
				else
					nextWidth = nextWidth.Between(NewSize.Width, AnimatedControl.Width - 1);

				var incHeight = (NewSize.Height - AnimatedControl.Height).Sign() == 1;
				var nextHeight = (int)(AnimatedControl.Height + ((NewSize.Height - AnimatedControl.Height) / SpeedModifier).Between(incHeight.If(3, -50), incHeight.If(50, -3)));

				if (incHeight)
					nextHeight = nextHeight.Between(AnimatedControl.Height + 1, NewSize.Height);
				else
					nextHeight = nextHeight.Between(NewSize.Height, AnimatedControl.Height - 1);

				AnimatedControl.TryInvoke(() =>
				{
					try
					{
						if (IgnoreHeight)
							AnimatedControl.Width = nextWidth;
						else if (IgnoreWidth)
							AnimatedControl.Height = nextHeight;
						else
							AnimatedControl.Size = new Size(nextWidth, nextHeight);

						var perc = 0D;

						if (!IgnoreWidth)
							perc += IgnoreHeight.If(2, 1) * incWidth.If(50, -50) * ((double)AnimatedControl.Width) / NewSize.Width;

						if (!IgnoreHeight)
							perc += IgnoreWidth.If(2, 1) * incHeight.If(50, -50) * ((double)AnimatedControl.Height) / NewSize.Height;

						OnAnimationTick?.Invoke(this, AnimatedControl, perc);
						if (perc == 100)
							OnEnd?.Invoke(this, AnimatedControl, perc);
					}
					catch { }
				});
			}
			catch { }

			if (!disposedValue && CheckIfEnded())
			{
				AnimatedControl?.TryInvoke(() =>
				{
					try
					{
						if (IgnoreHeight)
							AnimatedControl.Width = NewSize.Width;
						else if (IgnoreWidth)
							AnimatedControl.Height = NewSize.Height;
						else
							AnimatedControl.Size = NewSize;

						OnAnimationTick?.Invoke(this, AnimatedControl, 100);
						endAction?.Invoke();
					}
					catch { }
				});

				Dispose();
			}
		}

		private bool CheckIfEnded()
		{
			try
			{
				if (AnimatedControl != null && !IgnoreWidth)
				{
					if (Lazy)
					{
						if (Math.Abs(AnimatedControl.Width - NewSize.Width) > 2)
							return false;
					}
					else if (AnimatedControl.Width != NewSize.Width)
						return false;
				}

				if (AnimatedControl != null && !IgnoreHeight)
				{
					if (Lazy)
					{
						if (Math.Abs(AnimatedControl.Height - NewSize.Height) > 2)
							return false;
					}
					else if (AnimatedControl.Height != NewSize.Height)
						return false;
				}
			}
			catch { }

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
