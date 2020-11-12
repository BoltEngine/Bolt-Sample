using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

namespace Bolt.Samples.Voice
{
	[BoltGlobalBehaviour("Voice_Meeting")]
	public class BoltVoiceNetworkCallbacks : GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{
			var entity = BoltNetwork.Instantiate(BoltPrefabs.BoltVoicePlayer, Vector3.up, Quaternion.identity);
			entity.TakeControl();
		}
	}
}
