using UnityEngine;
using System;
using UdpKit;
using udpkit.platform.photon.photon;
using Bolt.Utils;

namespace Bolt.Samples.Photon.Simple
{
	public class PhotonInit : Bolt.GlobalEventListener
	{
		// helper enum and attribute to hold which mode application is running on
		enum State
		{
			SelectMode,
			ModeServer,
			ModeClient
		}
		State _state;

		public override void BoltStartBegin()
		{
			BoltNetwork.RegisterTokenClass<RoomProtocolToken>();
			BoltNetwork.RegisterTokenClass<ServerAcceptToken>();
			BoltNetwork.RegisterTokenClass<ServerConnectToken>();

			// Uncomment if you want to pass custom properties into your room
			// BoltNetwork.RegisterTokenClass<PhotonCloudRoomProperties>();
		}

		void Awake()
		{
			// Set Bolt to use Photon as transport layer
			// this will connect to Photon using config values from Bolt's settings window
			BoltLauncher.SetUdpPlatform(new PhotonPlatform());

			// Optionally, you may want to config the Photon transport layer programatically:
			//BoltLauncher.SetUdpPlatform(new PhotonPlatform(new PhotonPlatformConfig
			//{
			//    AppId = "<your-app-id>",
			//    RegionMaster = "<your-region>",
			//    UsePunchThrough = false // set to true, to use PunchThrough
			//}));
		}

		void OnGUI()
		{
			GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

			switch (_state)
			{
				// starting Bolt is the same regardless of the transport layer
				case State.SelectMode:
					if (GUILayout.Button("Start Server", GUILayout.ExpandWidth(true)))
					{
						BoltLauncher.StartServer();
						_state = State.ModeServer;
					}

					if (GUILayout.Button("Start Client", GUILayout.ExpandWidth(true)))
					{
						BoltLauncher.StartClient();
						_state = State.ModeClient;
					}

					break;

				// Publishing a session into the matchmaking server
				case State.ModeServer:
					if (BoltNetwork.IsRunning && BoltNetwork.IsServer)
					{
						if (GUILayout.Button("Publish HostInfo And Load Map", GUILayout.ExpandWidth(true)))
						{
							RoomProtocolToken token = new RoomProtocolToken()
							{
								ArbitraryData = "My DATA",
								password = "mysuperpass123"
							};

							// Uncomment if you want to pass custom properties into your room
							// This is just an example data
							//PhotonCloudRoomProperties token = new PhotonCloudRoomProperties();
							//properties.AddRoomProperty("t", 1);
							//properties.AddRoomProperty("m", 4);

							string matchName = "MyPhotonGame #" + UnityEngine.Random.Range(1, 100);

							BoltNetwork.SetServerInfo(matchName, token);
							BoltNetwork.LoadScene("Level1");
						}
					}
					break;
				// for the client, after Bolt is innitialized, we should see the list
				// of available sessions and join one of them
				case State.ModeClient:

					if (BoltNetwork.IsRunning && BoltNetwork.IsClient)
					{
						GUILayout.Label("Session List");

						foreach (var session in BoltNetwork.SessionList)
						{
							// Simple session
							UdpSession udpSession = session.Value as UdpSession;

							// Skip if is not a Photon session
							if (udpSession.Source != UdpSessionSource.Photon)
								continue;

							// Photon Session
							PhotonSession photonSession = udpSession as PhotonSession;

							string sessionDescription = String.Format("{0} / {1} ({2})",
								photonSession.Source, photonSession.HostName, photonSession.Id);

							RoomProtocolToken token = photonSession.GetProtocolToken() as RoomProtocolToken;

							if (token != null)
							{
								sessionDescription += String.Format(" :: {0}", token.ArbitraryData);
							}
							else
							{
								object value_t = -1;
								object value_m = -1;

								if (photonSession.Properties.ContainsKey("t"))
								{
									value_t = photonSession.Properties["t"];
								}

								if (photonSession.Properties.ContainsKey("m"))
								{
									value_t = photonSession.Properties["m"];
								}

								sessionDescription += String.Format(" :: {0}/{1}", value_t, value_m);
							}

							if (GUILayout.Button(sessionDescription, GUILayout.ExpandWidth(true)))
							{
								ServerConnectToken connectToken = new ServerConnectToken
								{
									data = "ConnectTokenData"
								};

								BoltNetwork.Connect(photonSession, connectToken);
							}
						}
					}
					break;
			}

			GUILayout.EndArea();
		}
	}
}