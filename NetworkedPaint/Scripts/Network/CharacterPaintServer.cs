using Photon.Bolt;

namespace Bolt.Samples.NetworkPaintStreamSample.Network
{
	[BoltGlobalBehaviour(BoltNetworkModes.Server, "NetworkedPaint_Game")]
	public class CharacterPaintServer : GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene, IProtocolToken token)
		{
			var entity = BoltNetwork.Instantiate(BoltPrefabs.CharacterPainter);
			entity.TakeControl();
		}
	}
}
