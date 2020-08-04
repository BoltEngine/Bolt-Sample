using UnityEngine;

namespace Bolt.Samples.GettingStarted
{
	public class RobotBehavior : Bolt.EntityBehaviour<IRobotState>
	{
		// private List<BoltEntity> boxes = new List<BoltEntity>();
		// private Color myColor = Random.ColorHSV();

		public override void Attached()
		{
			state.SetTransforms(state.Transform, transform);
			state.SetAnimator(GetComponent<Animator>());
			state.Animator.applyRootMotion = entity.IsOwner;
		}

		public void AddBox(BoltEntity box)
		{
			// boxes.Add(box);
			// box.SetParent(entity);
			// box.GetState<IInteractiveState>().Color = myColor;
		}

		public override void SimulateOwner()
		{
			var speed = state.Speed;
			var angularSpeed = state.AngularSpeed;

			if (Input.GetKey(KeyCode.W))
			{
				speed += 0.04f;
			}
			else
			{
				speed -= 0.04f;
			}

			if (Input.GetKey(KeyCode.A))
			{
				angularSpeed -= 0.025f;
			}
			else if (Input.GetKey(KeyCode.D))
			{
				angularSpeed += 0.025f;
			}
			else
			{
				if (angularSpeed < 0)
				{
					angularSpeed += 0.025f;
					angularSpeed = Mathf.Clamp(angularSpeed, -1f, 0);
				}
				else if (angularSpeed > 0)
				{
					angularSpeed -= 0.025f;
					angularSpeed = Mathf.Clamp(angularSpeed, 0, +1f);
				}
			}

			state.Speed = Mathf.Clamp(speed, 0f, 2.5f);
			state.AngularSpeed = Mathf.Clamp(angularSpeed, -2f, +2f);
		}
	}
}