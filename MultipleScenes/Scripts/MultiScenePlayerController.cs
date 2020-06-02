using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiScenePlayerController : Bolt.EntityEventListener<IMultiScenePlayer>
{
	[SerializeField] private Camera view;

	private const string VerticalAxisName = "Vertical";
	private const string HorizontalAxisName = "Horizontal";

	private float Speed = 30;
	private float MoveSpeed = 5;

	public override void Attached()
	{
		state.SetTransforms(state.Transform, transform);

		if (entity.IsOwner)
		{
			view.gameObject.SetActive(true);
		}
	}

	public override void SimulateOwner()
	{
		transform.Translate(MoveSpeed * Vector3.forward * Input.GetAxis(VerticalAxisName) * Time.deltaTime);
		transform.Rotate(Speed * Vector3.up * Input.GetAxis(HorizontalAxisName) * Time.deltaTime);
	}
}
