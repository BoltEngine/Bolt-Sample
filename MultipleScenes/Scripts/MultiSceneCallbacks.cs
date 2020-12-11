using Photon.Bolt;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server | BoltNetworkModes.Client, "InGame_Main")]
public class MultiSceneCallbacks : GlobalEventListener
{
	public override void SceneLoadLocalDone(string scene, IProtocolToken token)
	{
		// When the local player enters the Game Scene, it will instantiate it's own Player Character
		BoltNetwork.Instantiate(BoltPrefabs.MultiScenePlayer, Vector3.zero, Quaternion.identity);
	}
}
