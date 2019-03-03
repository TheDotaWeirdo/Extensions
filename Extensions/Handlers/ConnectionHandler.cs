using Timer = System.Timers.Timer;

namespace Extensions
{
	public enum ConnectionState { Connected, Disconnected }

	public delegate void ConnectionEventHandler(ConnectionState newState);

	public static class ConnectionHandler
	{
		public static event ConnectionEventHandler ConnectionChanged;
		public static event ConnectionEventHandler Connected;

		private static Timer checkTimer;

		private static ConnectionState state = ConnectionState.Disconnected;

		public static ConnectionState State
		{
			get => state;
			private set
			{
				if (value != state)
				{
					state = value;
					ConnectionChanged?.Invoke(value);
				}
			}
		}

		public static void Start()
		{
			checkTimer = new Timer(5000);

			checkTimer.Start();

			checkTimer.Elapsed += (s, e) =>
			{
				State = getConnectionState();

				if (State == ConnectionState.Connected)
				{
					Connected?.Invoke(state);
					Connected = null;
				}
			};

			State = getConnectionState();
		}

		private static ConnectionState getConnectionState()
		{
			try
			{
				using (var client = new System.Net.WebClient())
				{
					using (client.OpenRead("http://clients3.google.com/generate_204"))
					{
						return ConnectionState.Connected;
					}
				}
			}
			catch
			{
				return ConnectionState.Disconnected;
			}
		}
	}
}
