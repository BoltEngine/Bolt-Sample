#if ENABLE_PLAYFABSERVER_API

using System;
using System.Collections.Generic;
using System.Linq;
using Bolt.Matchmaking;
using Bolt.Photon;
using PlayFab.MultiplayerAgent.Model;
using UdpKit;
using UdpKit.Platform;
using UnityEngine;

namespace Bolt.Samples.PlayFab
{
	/// <summary>
	/// Bolt Related Calls
	///
	/// This class contains the calls and handles to Photon Bolt SDK
	/// </summary>
	public partial class PlayFabHeadlessServer
	{
		/// <summary>
		/// If any client has connected
		/// </summary>
		private bool _hadClients = false;

		private void LateUpdate()
		{
			if (BoltNetwork.Frame % 60 == 0 && // every 60 frame
					_hadClients && // someone has connected before
					BoltNetwork.Clients.Any() == false) // and there is no one now
			{
				BoltLog.Error("Shutting down: not clients left");
				BoltNetwork.Shutdown();
			}
		}

		/// <summary>
		/// Register the PhotonRoomProperties Token to be used on the Session creation
		/// </summary>
		public override void BoltStartBegin()
		{
			// Register PhotonRoomProperties to be used when creating the Photon Session
			BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
		}

		/// <summary>
		/// If running as Server, creates the session and load the Game scene
		/// </summary>
		public override void BoltStartDone()
		{
			if (BoltNetwork.IsServer)
			{
				_connectedPlayers = new List<ConnectedPlayer>();

				// Create some room custom properties
				PhotonRoomProperties roomProperties = new PhotonRoomProperties();

				roomProperties.AddRoomProperty("m", config.Map);
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

		public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
		{
			registerDoneCallback(() =>
			{
				// Quit Unity
				Application.Quit();
			});
		}

		/// <summary>
		/// Start this peer as the Game Server
		/// </summary>
		private void OnServerActive()
		{
			if (BoltNetwork.IsRunning) { return; }

			try
			{
				// In order to start the server property when running on the PlayFab stack, it's necessary
				// to setup the local port where the server will listen and suppress the STUN request by passing
				// the binding information provided by PlayFab
				if (BuildBindingInfo(out BindingInfo info))
				{
					// Override the STUN information sent by this peer, in other words, the public IP:PORT of this
					// instance. This information is gathered directly from the PlayFab stack, that provides statically
					// the binding data of each Virtual Machine
					BoltLauncher.SetUdpPlatform(new PhotonPlatform(new PhotonPlatformConfig()
					{
						ForceExternalEndPoint = info.externalInfo
					}));

					// Set the Server port using the information from the binding configuration
					BoltLauncher.StartServer(info.internalServerPort);
				}
				else // Shutdown if the binding info was not found
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

		/// <summary>
		/// Connected Handler, update the number of connected players to the Server in the PlayFab stack
		/// </summary>
		public override void Connected(BoltConnection connection)
		{
			if (BoltNetwork.IsServer)
			{
				// If someone has connected, here we know
				_hadClients = true;

				OnPlayerAdded(connection.RemoteEndPoint.ToString());
			}
		}

		/// <summary>
		/// Disconnected Handler, update the number of connected players to the Server in the PlayFab stack
		/// </summary>
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
