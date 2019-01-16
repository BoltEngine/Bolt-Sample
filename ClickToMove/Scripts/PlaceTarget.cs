using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bolt.Samples.ClickToMove
{
	public class PlaceTarget : MonoBehaviour
	{
		public float surfaceOffset = 0.2f;
		public event Action<Transform> UpdateTarget;

		Vector3 lastPosition = Vector3.zero;

		private void Update()
		{
			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (!Physics.Raycast(ray, out hit))
			{
				return;
			}

			transform.position = hit.point + hit.normal * surfaceOffset;

			if (lastPosition != transform.position)
			{
				lastPosition = transform.position;

				if (UpdateTarget != null)
				{
					UpdateTarget(transform);
				}
			}
		}
	}
}