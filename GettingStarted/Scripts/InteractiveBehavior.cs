using UnityEngine;
using System.Linq;
using Photon.Bolt;

namespace Bolt.Samples.GettingStarted
{
	public class InteractiveBehavior : EntityBehaviour<IInteractiveState>
	{
		[Range(2, 10)]
		public int interactiveRadius;

		private Color activeColor;
		private Color normalColor;

		private void Awake()
		{
			activeColor = Color.blue;
			normalColor = GetComponent<Renderer>().material.color;
		}

		public override void Attached()
		{
			state.SetTransforms(state.Transform, transform);

			if (entity.IsOwner)
			{
				state.Color = normalColor;
			}

			state.AddCallback("Color", () =>
			{
				GetComponent<MeshRenderer>().material.color = state.Color;
			});
		}

		private void Update()
		{
			if (entity.IsAttached && entity.IsOwner)
			{
				var nearbyPlayer = (from player in GameObject.FindGameObjectsWithTag("Player")
														where Vector3.Distance(transform.position, player.transform.position) < interactiveRadius
														select player).FirstOrDefault();

				if (nearbyPlayer != null)
				{
					// var robot = nearbyPlayer.GetComponent<RobotBehavior>();
					// if (robot != null)
					// {
					// 	robot.AddBox(entity);
					// }

					state.Color = activeColor;
				}
				else
				{
					state.Color = normalColor;
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, interactiveRadius);
		}
	}
}
