using System.Collections.Generic;
using Bolt.Matchmaking;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.Unity;
using UdpKit;
using UnityEngine;

namespace Bolt.Samples.Voice
{
	[RequireComponent(typeof(VoiceConnection), typeof(Recorder))]
	public class BoltVoiceBridge : Bolt.GlobalEventListener, IConnectionCallbacks, IMatchmakingCallbacks
	{
		[SerializeField] private GameObject BoltSpeakerPrefab;
		[SerializeField] private bool autoConnect = true;


		public static BoltVoiceBridge Instance;

		private VoiceConnection voiceConnection;
		private Recorder voiceRecorder;
		private byte voiceRecorderDefaultGroup;
		private Dictionary<int, BoltVoiceSpeakerController> speakerRegistry;

		public string RoomName
		{
			get
			{
				var session = BoltMatchmaking.CurrentSession;
				var sessionName = session.HostName;
				return string.Format("{0}_voice", sessionName);
			}
		}

		public string Region
		{
			get
			{
				return BoltMatchmaking.CurrentMetadata["Region"] as string;
			}
		}

		public int LocalPlayerID
		{
			get
			{
				if (IsConnected)
				{
					return this.voiceConnection.Client.LocalPlayer.ActorNumber;
				}

				return -1;
			}
		}

		private EnterRoomParams enterRoomParams
		{
			get
			{
				return new EnterRoomParams
				{
					RoomName = RoomName,
					RoomOptions = new RoomOptions()
					{
						IsVisible = false,
						PublishUserId = false,
					}
				};
			}
		}

		public bool IsConnected { get { return voiceConnection.Client.IsConnected; } }

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

			voiceConnection = GetComponent<VoiceConnection>();
			voiceConnection.SpeakerFactory = this.CustomBoltSpeakerFactory;

			voiceRecorder = GetComponent<Recorder>();
			voiceRecorderDefaultGroup = voiceRecorder.InterestGroup;

			speakerRegistry = new Dictionary<int, BoltVoiceSpeakerController>();
		}

		#region Voice Events

		protected Speaker CustomBoltSpeakerFactory(int playerId, byte voiceId, object userData)
		{
			BoltLog.Info("[BoltVoiceBridge] SpeakerFactory for Player {0}", playerId);

			var speakerInstance = Instantiate(BoltSpeakerPrefab);

			var speaker = speakerInstance.GetComponent<Speaker>();
			var speakerController = speakerInstance.GetComponent<BoltVoiceSpeakerController>();

			speakerController.PlayerID = playerId;

			speakerRegistry.Add(playerId, speakerController);

			return speaker;
		}

		#endregion

		#region Bolt Events

		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{
			if (BoltNetwork.IsServer || BoltNetwork.IsClient)
			{
				voiceConnection.Client.AddCallbackTarget(this);
				if (this.autoConnect)
				{
					this.ConnectNow();
				}
			}
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
		{
			if (BoltNetwork.IsServer || BoltNetwork.IsClient)
			{
				voiceConnection.Client.RemoveCallbackTarget(this);
				voiceConnection.Client.Disconnect();
			}
		}

		#endregion

		#region Utils

		public void ChangeVoiceGround(int groupID)
		{
			if (voiceRecorder != null)
			{
				voiceConnection.Client.OpChangeGroups(
					new byte[0],
					new byte[] { (byte) groupID }
				);

				voiceRecorder.InterestGroup = (byte)groupID;

				BoltLog.Info("[BoltVoiceBridge] InterestGroup changed to {0}", voiceRecorder.InterestGroup);
			}
		}

		public void ResetVoiceGroup()
		{
			if (voiceRecorder != null)
			{
				voiceConnection.Client.OpChangeGroups(
					new byte[0],
					new byte[] { voiceRecorderDefaultGroup }
				);

				voiceRecorder.InterestGroup = voiceRecorderDefaultGroup;

				BoltLog.Info("[BoltVoiceBridge] InterestGroup changed to {0}", voiceRecorder.InterestGroup);
			}
		}

		public bool GetSpeaker(int playerID, out GameObject speaker)
		{
			BoltVoiceSpeakerController controller;
			var found = speakerRegistry.TryGetValue(playerID, out controller);

			if (found)
			{
				speaker = controller.gameObject;
				return true;
			}

			speaker = null;
			return false;
		}

		public void ConnectNow()
		{
			BoltLog.Info("[BoltVoiceBridge] Starting Voice connection...");

			var customSettings = new AppSettings();
			voiceConnection.Settings.CopyTo(customSettings);

			// Connect to the same Region as Bolt has connected
			customSettings.FixedRegion = Region;

			if (voiceConnection.ConnectUsingSettings(customSettings))
			{
				BoltLog.Info("[BoltVoiceBridge] Connecting to Region {0}", customSettings.FixedRegion);
			}
			else
			{
				BoltLog.Error("[BoltVoiceBridge] Not able to connect");
			}
		}

		#endregion

		#region MatchmakingCallbacks

		public void OnCreatedRoom()
		{
			BoltLog.Info("[BoltVoiceBridge] Room Created {0}", voiceConnection.Client.CurrentRoom.Name);
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
			BoltLog.Error("[BoltVoiceBridge] OnCreateRoomFailed errorCode={0} errorMessage={1}", returnCode, message);
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList) { }

		public void OnJoinedRoom()
		{
			if (this.voiceConnection.PrimaryRecorder == null)
			{
				this.voiceConnection.PrimaryRecorder = this.gameObject.AddComponent<Recorder>();
			}

			var actorNumber = this.voiceConnection.Client.LocalPlayer.ActorNumber;

			BoltLog.Info("[BoltVoiceBridge] Joined Room as Player {0}", actorNumber);

			ResetVoiceGroup();
		}

		public void OnJoinRandomFailed(short returnCode, string message)
		{
			BoltLog.Error("[BoltVoiceBridge] OnJoinRandomFailed errorCode={0} errorMessage={1}", returnCode, message);
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
			BoltLog.Error("[BoltVoiceBridge] OnJoinRoomFailed roomName={0} errorCode={1} errorMessage={2}", this.RoomName, returnCode, message);
		}

		public void OnLeftRoom()
		{
			BoltLog.Info("[BoltVoiceBridge] Left Room");
		}

		#endregion

		#region ConnectionCallbacks

		public void OnConnected()
		{
			BoltLog.Info("[BoltVoiceBridge] Connected");
		}

		public void OnConnectedToMaster()
		{
			this.voiceConnection.Client.OpJoinOrCreateRoom(this.enterRoomParams);
		}

		public void OnDisconnected(DisconnectCause cause)
		{
			if (cause == DisconnectCause.None || cause == DisconnectCause.DisconnectByClientLogic)
			{
				return;
			}
			BoltLog.Error("[BoltVoiceBridge] OnDisconnected cause={0}", cause);
		}

		public void OnRegionListReceived(RegionHandler regionHandler) { }

		public void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }

		public void OnCustomAuthenticationFailed(string debugMessage) { }

		#endregion

	}
}
