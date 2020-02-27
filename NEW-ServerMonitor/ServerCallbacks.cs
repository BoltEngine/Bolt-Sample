using UnityEngine;
using System.Collections;
using Bolt;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "Level1")]
public class ServerCallbacks : Bolt.GlobalEventListener
{
	public override void SceneLoadLocalDone(string scene, IProtocolToken token)
	{
		// instantiate server monitor stuff
		Instantiate(Resources.Load("ServerMonitor"));
	}
}
