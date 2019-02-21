using System.Collections;
using UnityEngine;

public class TPCspawner : Bolt.GlobalEventListener
{
	// Use this for initialization
	public override void SceneLoadLocalDone(string scene)
	{
		BoltNetwork.Instantiate(BoltPrefabs.EthanClientAuth, new Vector3(0, -1f, 0), Quaternion.identity);
	}
}