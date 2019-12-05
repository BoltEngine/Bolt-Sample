#if ENABLE_PLAYFABSERVER_API

using System;
using System.Collections.Generic;
using Bolt.Matchmaking;
using Bolt.Photon;
using PlayFab.MultiplayerAgent.Model;
using UnityEngine;

namespace Bolt.Samples.PlayFab
{
	/// <summary>
	/// Bolt Related Calls
	/// </summary>
	public partial class PlayFabHeadlessServer
	{
		public override void BoltStartBegin()
		{
			// Register PhotonRoomProperties to be used when creating the Photon Session
			BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
		}

		public override void BoltStartDone()
		{
			if (BoltNetwork.IsServer)
			{
				_connectedPlayers = new List<ConnectedPlayer>();

				// Create some room custom properties
				PhotonRoomProperties roomProperties = new PhotonRoomProperties();

				roomProperties["m"] = config.Map;
				roomProperties.IsOpen = true;
				roomProperties.IsVisible = true;

				// Create the Photon Room
				BoltMatchmaking.CreateSession(
					sessionID: Guid.NewGuid().ToString(),
					token: roomProperties,
					sceneToLoad: config.Map
				);
			}
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback)
		{
			registerDoneCallback(() =>
			{
				// Quit Unity
				Application.Quit();
			});
		}

		private void OnServerActive()
		{
			if (BoltNetwork.IsRunning) { return; }

			try
			{
				BindingInfo info;
				if (BuildBindingInfo(out info))
				{
					BoltLauncher.SetUdpPlatform(new PhotonPlatform(new PhotonPlatformConfig()
					{
						ForceExternalEndPoint = info.externalInfo
					}));

					BoltLauncher.StartServer(info.internalServerPort);
				}
				else
				{
					BoltLog.Error(MessageInvalidBinding);
					OnShutdown();
				}
			}
			catch (Exception ex)
			{
				BoltLog.Error(MessageExceptionServer);
				BoltLog.Exception(ex);
				OnShutdown();
			}
		}

		private void OnShutdown()
		{
			BoltLog.Info(MessageBoltShutdown);
			BoltNetwork.Shutdown();
		}

		public override void Connected(BoltConnection connection)
		{
			if (BoltNetwork.IsServer)
			{
				OnPlayerAdded(connection.RemoteEndPoint.ToString());
			}
		}

		public override void Disconnected(BoltConnection connection)
		{
			if (BoltNetwork.IsServer)
			{
				OnPlayerRemoved(connection.RemoteEndPoint.ToString());
			}
		}
	}
}

#endif