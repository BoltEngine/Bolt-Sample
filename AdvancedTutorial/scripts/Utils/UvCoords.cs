using UnityEngine;

namespace Bolt.AdvancedTutorial
{
	[RequireComponent (typeof(MeshFilter))]
	public class UvCoords : MonoBehaviour
	{
		[SerializeField]
		Vector2[] uv = new Vector2[4];

		void Start ()
		{
			GetComponent<MeshFilter> ().mesh.uv = uv;
		}
	}
}