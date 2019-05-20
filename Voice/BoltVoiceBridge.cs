#if ENABLE_VOICE

using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;

namespace Bolt.Samples.Voice
{
	[RequireComponent(typeof(VoiceConnection))]
	public class BoltVoiceBridge : Bolt.GlobalEventListener
	{
		private VoiceConnection voiceConnection;

		public bool RandomRoom = true;

		[SerializeField]
		private bool autoConnect = true;

		[SerializeField]
		private bool autoTransmit = true;

		public string RoomName;

		private RoomOptions roomOptions = new RoomOptions();
		private TypedLobby typedLobby = TypedLobby.Default;

		public bool IsConnected { get { return voiceConnection.Client.IsConnected; } }

		private void Awake()
		{
			voiceConnection = GetComponent<VoiceConnection>();
		}

		public override void BoltStartDone()
		{
			voiceConnection.Client.AddCallbackTarget(this);
			if (this.autoConnect)
			{
				this.ConnectNow();
			}
		}

		public override void BoltShutdownBegin(AddCallback registerDoneCallback)
		{
			voiceConnection.Client.RemoveCallbackTarget(this);
		}

		public void ConnectNow()
		{
			Debug.Log("ConnectAndJoin.ConnectNow() will now call: VoiceConnection.ConnectUsingSettings().");
			voiceConnection.ConnectUsingSettings();
		}

		// Use this for initialization
		void Start() { }

		// Update is called once per frame
		void Update() { }
	}
}

#endif