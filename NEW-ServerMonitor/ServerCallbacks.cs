using UnityEngine;
using Photon.Bolt;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "Level1")]
public class ServerCallbacks : GlobalEventListener
{
	public override void SceneLoadLocalDone(string scene, IProtocolToken token)
	{
		// instantiate server monitor stuff
		Instantiate(Resources.Load("ServerMonitor"));
	}
}
