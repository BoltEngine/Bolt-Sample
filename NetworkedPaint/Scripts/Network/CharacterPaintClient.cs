namespace Bolt.Samples.NetworkPaintStreamSample.Network
{
	[BoltGlobalBehaviour(BoltNetworkModes.Client, "NetworkedPaint_Game")]
	public class CharacterPaintClient : Bolt.GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene, Bolt.IProtocolToken token)
		{
			var entity = BoltNetwork.Instantiate(BoltPrefabs.CharacterPainter);
			entity.TakeControl();
		}
	}
}
