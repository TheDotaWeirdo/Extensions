using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
	public class ActionQueue
	{
		private static ulong IdList = 0;
		private List<ulong> queueList = new List<ulong>();
		public ulong ID { get; private set; }

		public ulong GetTicket(int lifespan)
		{
			queueList.Add(IdList++);
			var ticket = queueList.Last();
			var T = new System.Timers.Timer(lifespan) { AutoReset = false };
			T.Elapsed += (s, e) => queueList.Remove(ticket);
			T.Start();
			return ticket;
		}

		public bool IsTurn(ulong ticket)
		{
			try
			{ return queueList.Count == 0 || queueList.Min() == ticket; }
			catch (InvalidOperationException) { return IsTurn(ticket); }
		}

		public void ReleaseTicket(ulong ticket)
		{
			queueList.Remove(ticket);
		}
	}
}