using Photon.Bolt;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
	private Rect _shutdownButtonRect;

	void Start()
	{
		_shutdownButtonRect = new Rect(Screen.width - 200, Screen.height - 100, 190, 60);
	}

	private void OnGUI()
	{
		if (GUI.Button(_shutdownButtonRect, "Shutdown") && BoltNetwork.IsRunning)
		{
			BoltNetwork.Shutdown();
		}
	}
}
