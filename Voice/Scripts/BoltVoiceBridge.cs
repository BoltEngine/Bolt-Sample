using System;
using System.Collections.Generic;
using Bolt.Matchmaking;
using UdpKit;
using UnityEngine;

#if BOLT_VOICE_SAMPLE
using Photon.Realtime;
using Photon.Voice.Unity;
#endif

namespace Bolt.Samples.Voice
{
	/// <summary>
	/// Class responsible for managing all the Connection with the Voice Rooms and also the speakers for each player
	///
	/// It will create a new custom speaker when a new player enters the game, making it available for later use.
	/// </summary>
	public partial class BoltVoiceBridge : Bolt.GlobalEventListener
	{
		// ------------ UNITY MEMBERS ---------------------------------------------------------------------------------------

		/// <summary>
		/// Custom Speaker for Bolt
		/// </summary>
		[SerializeField] private GameObject BoltSpeakerPrefab;

		// ------------ PUBLIC MEMBERS --------------------------------------------------------------------------------------

		/// <summary>
		/// Singleton Instance
		/// </summary>
		public static BoltVoiceBridge Instance;

		// ------------ PRIVATE MEMBERS -------------------------------------------------------------------------------------

		/// <summary>
		/// Mapping for Spekers and Players
		///
		/// This is used to retrieve the custom Speaker Game Object and parent it to the player
		/// </summary>
		private Dictionary<int, BoltVoiceSpeakerController> speakerRegistry;

		/// <summary>
		/// Builds the Room Name where the Voice Client must enter.
		/// This name is based on the Bolt game session name that the player is already in.
		/// </summary>
		public string RoomName
		{
			get
			{
				var session = BoltMatchmaking.CurrentSession;
				var sessionName = session.HostName; // Bolt Session Name
				return string.Format("{0}_voice", sessionName); // include a '_voice' to create a shadow room
			}
		}

		/// <summary>
		/// Retrieve the current region the Player is already connected with Bolt
		public string Region
		{
			get
			{
				return BoltMatchmaking.CurrentMetadata["Region"] as string;
			}
		}

		private void Awake()
		{
			// Simple Singleton
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(this.gameObject);
			}
		}

#region Bolt Events

		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{
			if (BoltNetwork.IsServer || BoltNetwork.IsClient)
			{
				SetupAndConnect();
			}
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
		{
			if (BoltNetwork.IsServer || BoltNetwork.IsClient)
			{
				ShutdownVoiceConnection();
			}
		}

#endregion

		private void SetupAndConnect()
		{
#if BOLT_VOICE_SAMPLE
			// Registers this class to receive Photon callbacks
			voiceConnection.Client.AddCallbackTarget(this);

			// Auto automatically
			this.ConnectNow();
#else
			BoltLog.Error("[BoltVoiceBridge] Unable to initilize the Voice connection, please insert 'BOLT_VOICE_SAMPLE' into your Scripting Symbols");
#endif
		}

		private void ShutdownVoiceConnection()
		{
#if BOLT_VOICE_SAMPLE
			// Remove this class from the listeners list
			voiceConnection.Client.RemoveCallbackTarget(this);

			// Disconnect the Voice client when Bolt start to shutdown
			voiceConnection.Client.Disconnect();
#endif
		}
	}
}
