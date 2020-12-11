using Photon.Bolt;

namespace Bolt.Samples.NetworkPaintStreamSample.Network
{
	[BoltGlobalBehaviour(BoltNetworkModes.Client, "NetworkedPaint_Game")]
	public class CharacterPaintClient : GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{
			var entity = BoltNetwork.Instantiate(BoltPrefabs.CharacterPainter);
			entity.TakeControl();
		}
	}
}
