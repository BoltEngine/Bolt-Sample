using Bolt;
using UnityEngine;

public class InteractiveSpawner : Bolt.GlobalEventListener
{
	[SerializeField] private BoltEntity prefabEntity;

	private void Start()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	public override void SceneLoadLocalDone(string scene, IProtocolToken token)
	{
		if (BoltNetwork.IsServer)
		{
			BoltNetwork.Instantiate(prefabEntity.gameObject, transform.position, Quaternion.identity);
		}
	}
}
