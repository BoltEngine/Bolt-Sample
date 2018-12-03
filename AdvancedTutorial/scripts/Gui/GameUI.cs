using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{

	public class GameUI : BoltSingletonPrefab<GameUI>
	{

		public GameCrosshair crosshair {
			get { return GetComponentInChildren<GameCrosshair> (); }
		}

		void Start ()
		{
			if (!GetComponent<Camera> ()) {
				gameObject.AddComponent<Camera> ();
			}

			GetComponent<Camera> ().orthographic = true;
			GetComponent<Camera> ().cullingMask = 1 << LayerMask.NameToLayer ("GUI");
			GetComponent<Camera> ().nearClipPlane = 0;
			GetComponent<Camera> ().farClipPlane = 500f;
			GetComponent<Camera> ().useOcclusionCulling = false;
			GetComponent<Camera> ().depth = 1;
			GetComponent<Camera> ().clearFlags = CameraClearFlags.Depth;

			transform.position = new Vector3 (0, 0, -250f);
		}

		void Update ()
		{
			if (GetComponent<Camera> ()) {
				GetComponent<Camera> ().orthographicSize = Screen.height / 2;
			}
		}
	}

}
