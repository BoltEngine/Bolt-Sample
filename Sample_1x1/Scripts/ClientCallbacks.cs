using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FailedToJoin
{
	[BoltGlobalBehaviour(BoltNetworkModes.Client, "Game")]
	public class ClientCallbacks : Bolt.GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene)
		{
			Camera.main.backgroundColor = Color.green;
		
			Debug.LogFormat("Scene Load Done at {0}", scene);
		}
	}
}