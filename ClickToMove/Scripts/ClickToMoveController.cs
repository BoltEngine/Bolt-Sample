using Bolt;
using Photon.Bolt;
using UnityEngine;

namespace Bolt.Samples.ClickToMove
{
	public class ClickToMoveController : EntityEventListener<IClickToMoveState>
	{
		public Transform destination;

		public override void Attached()
		{
			state.SetTransforms(state.Transform, transform);
			state.SetAnimator(GetComponentInChildren<Animator>());
		}

		public override void SimulateController()
		{
			if (destination != null)
			{
				IClickToMoveCommandInput input = ClickToMoveCommand.Create();
				input.click = destination.position;
				entity.QueueInput(input);
			}
		}

		public override void ControlGained()
		{
			var placeTarget = GameObject.Find("TargetPointer").GetComponent<PlaceTarget>();

			placeTarget.UpdateTarget += (newTarget) =>
			{
				destination = newTarget;
			};
		}

		public override void ExecuteCommand(Command command, bool resetState)
		{
			ClickToMoveCommand cmd = (ClickToMoveCommand)command;

			if (resetState)
			{
				//owner has sent a correction to the controller
				transform.position = cmd.Result.position;
			}
			else
			{
				if (cmd.Input.click != Vector3.zero)
				{
					gameObject.SendMessage("SetTarget", cmd.Input.click);
				}

				cmd.Result.position = transform.position;
			}
		}
	}
}
