using UnityEngine;

namespace Bolt.Samples.Voice
{
	[BoltGlobalBehaviour("Voice_Meeting")]
	public class BoltVoiceNetworkCallbacks : GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{
			// Create a new Bolt Voice Player
			var entity = BoltNetwork.Instantiate(BoltPrefabs.BoltVoicePlayer, Vector3.up, Quaternion.identity);
		}
	}
}
