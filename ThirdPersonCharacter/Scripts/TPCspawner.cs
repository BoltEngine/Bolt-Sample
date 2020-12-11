using Photon.Bolt;
using UnityEngine;

public class TPCspawner : GlobalEventListener
{
	// Use this for initialization
	public override void SceneLoadLocalDone(string scene, IProtocolToken token)
	{
		BoltNetwork.Instantiate(BoltPrefabs.EthanClientAuth, new Vector3(0, -1f, 0), Quaternion.identity);
	}
}
