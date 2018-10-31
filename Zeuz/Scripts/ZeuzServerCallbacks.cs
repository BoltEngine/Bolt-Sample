namespace Bolt.Samples.Zeuz
{
	using System;
	using UdpKit;
	using Bolt.zeuz;

	public class ZeuzServerCallbacks : GlobalEventListener
	{
		//========== PRIVATE MEMBERS ===================================================================================

		private string m_Map;
		
		//========== PUBLIC METHODS ====================================================================================

		public override void Disconnected(BoltConnection connection)
		{
			foreach(BoltConnection client in BoltNetwork.Connections)
			{
				if(connection != client)
				{
					return;
				}
			}
			
			// Deinitialize zeuz and unreserve service when last client disconnected
			// This should be called when the game is over and you don't need this game server instance anymore

			Zeuz.Deinitialize(true);
		}
		
		//========== PRIVATE METHODS ===================================================================================

		private void Awake()
		{
			m_Map = GetArg("-map") ?? "Level1";
			
			// Initialize zeuz and preventively unreserve service
			// This also loads settings to run game server and identify gameplay - IP, Port, Server Group ID, Game Profile ID

			Zeuz.Initialize(true);
		}

		private void Start()
		{
			AdvancedTutorial.ServerCallbacks.ListenServer = false;

			// Start listening on IP + Port provided by zeuz, and load the Map received as command line argument (can be different for each profile defined in zeuz dashboard)

			BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Parse(Zeuz.IP), (ushort)Zeuz.GamePort), m_Map);
			DontDestroyOnLoad(this);
		}

		private static string GetArg(string name)
		{
			string[] args = Environment.GetCommandLineArgs();
			for(int i = 0; i < args.Length; ++i)
			{
				if(args[i] == name && args.Length > i + 1)
				{
					return args[i + 1];
				}
			}

			return null;
		}
	}
}
