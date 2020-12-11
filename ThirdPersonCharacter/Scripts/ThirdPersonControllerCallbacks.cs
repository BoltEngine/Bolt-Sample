using Photon.Bolt;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "serverAuthTPC")]
public class ThirdPersonControllerCallbacks : GlobalEventListener
{
	public override void SceneLoadLocalDone(string scene, IProtocolToken token)
	{
		BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.EthanServerAuth, new Vector3(0, -1f, 0), Quaternion.identity);
		player.TakeControl();
	}

	public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
	{
		BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.EthanServerAuth, new Vector3(0, -1f, 0), Quaternion.identity);
		player.AssignControl(connection);
	}
}
