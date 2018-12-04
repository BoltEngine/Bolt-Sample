using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "Level1")]
public class ServerCallbacks : Bolt.GlobalEventListener
{
	public override void SceneLoadLocalDone(string scene)
	{
		// instantiate server monitor stuff
		Instantiate(Resources.Load("ServerMonitor"));
	}
}
