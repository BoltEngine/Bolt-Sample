using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{
	public class LineFade : MonoBehaviour
	{
		[SerializeField]
		Color color;

		LineRenderer lr;

		void Start ()
		{
			lr = GetComponent<LineRenderer> ();
		}

		void Update ()
		{
			// move towards zero
			color.a = Mathf.Lerp (color.a, 0, Time.deltaTime * 10f);

			// update color
			//lr.SetColors (color, color);
			lr.startColor = color;
			lr.endColor = color;
		}
	}
}
