using System.Collections;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "serverAuthTPC")]
public class ThirdPersonControllerCallbacks : Bolt.GlobalEventListener
{
	public override void SceneLoadLocalDone(string scene)
	{
		BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.EthanServerAuth, new Vector3(0, -1f, 0), Quaternion.identity);
		player.TakeControl();
	}

	public override void SceneLoadRemoteDone(BoltConnection connection)
	{
		BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.EthanServerAuth, new Vector3(0, -1f, 0), Quaternion.identity);
		player.AssignControl(connection);
	}
}