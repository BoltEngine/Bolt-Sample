using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bolt.Samples.MoveAndShoot
{
	[RequireComponent(typeof(CharacterController))]
	public class MoveAndShoot_PlayerController : Bolt.EntityEventListener<IMoveAndShootPlayer>
	{
		private struct PlayerInput
		{
			public Vector3 _dir;
			public float _angle;
			public bool? _fired;
		}

		[Serializable]
		private class WeaponData
		{
			public int fireRate;
			public ParticleSystem fx;
			public int amount;

			internal int nextFireFrame;
			internal Action triggerState;
		}

		[SerializeField] private float moveSpeed = 4f;
		[SerializeField] private Transform view;
		[SerializeField] private LayerMask groundMask;
		[SerializeField] private WeaponData weaponDamage;
		[SerializeField] private WeaponData weaponHeal;

		private Camera _camera;
		private CharacterController _cc;
		private float _gravityValue = -9.81f * 4;
		private Vector3 _localVelocity;

		private PlayerInput _input;

		private void Awake()
		{
			_cc = GetComponent<CharacterController>();
			_camera = Camera.main;
		}

		private void Update()
		{
			PollInput();
		}

		public override void Attached()
		{
			state.SetTransforms(state.Transform, transform, view);

			if (entity.IsOwner)
			{
				state.Team = 1;
				state.Health = 100;
			}

			state.OnFireDamage += OnFireDamageHandler;
			state.OnFireHeal += OnFireHeadHandler;

			if (entity.IsControllerOrOwner)
			{
				weaponDamage.triggerState = () => { state.FireDamage(); };
				weaponHeal.triggerState = () => { state.FireHeal(); };
			}
		}

		public override void ControlGained()
		{
			SmoothCameraController.ChangeTarget(transform);
		}

		public override void SimulateController()
		{
			var cmd = MoveAndShootMoveCommand.Create();

			cmd.Direction = _input._dir;
			cmd.Yaw = _input._angle;

			entity.QueueInput(cmd);

			if (_input._fired.HasValue)
			{
				var fireCommandInput = MoveAndShootFireCommand.Create();
				fireCommandInput.Type = _input._fired.Value;

				entity.QueueInput(fireCommandInput);

				// reset fire input
				_input._fired = null;
			}
		}

		public override void ExecuteCommand(Command command, bool resetState)
		{
			var moveCommand = command as MoveAndShootMoveCommand;
			if (moveCommand != null)
			{
				HandleMoveCommand(moveCommand, resetState);
			}

			var fireCommand = command as MoveAndShootFireCommand;
			if (fireCommand != null)
			{
				HandleFireCommand(fireCommand, resetState);
			}
		}

		private void HandleMoveCommand(MoveAndShootMoveCommand cmd, bool reset)
		{
			if (reset)
			{
				SetState(cmd.Result.Position, cmd.Result.Velocity, cmd.Result.Yaw);
			}
			else
			{
				Vector3 resultPosition;
				Vector3 resultVelocity;
				float resultYaw;

				Move(cmd.Input.Direction, cmd.Input.Yaw, out resultPosition, out resultVelocity, out resultYaw);

				cmd.Result.Position = resultPosition;
				cmd.Result.Velocity = resultVelocity;
				cmd.Result.Yaw = resultYaw;
			}
		}

		private void HandleFireCommand(MoveAndShootFireCommand fireCommand, bool reset)
		{
			if (reset)
			{
				SetState(fireCommand.Result.Position, Vector3.zero, fireCommand.Result.Yaw);
			}
			else if (fireCommand.IsFirstExecution)
			{
				var inputType = fireCommand.Input.Type;
				var weapon = inputType ? weaponDamage : weaponHeal;

				if (weapon.nextFireFrame <= BoltNetwork.ServerFrame)
				{
					weapon.nextFireFrame = BoltNetwork.ServerFrame + weapon.fireRate;

					if (weapon.triggerState != null) { weapon.triggerState.Invoke(); }

					if (entity.IsOwner)
					{
						var fireOrigin = weapon.fx.transform;
						var fireOriginForward = fireOrigin.forward;
						Vector3 pos = fireOrigin.position + (fireOriginForward * 0.5f);
						// Quaternion look = fireOrigin.rotation;

						Debug.DrawRay(pos, fireOriginForward * 10, Color.yellow);

						using (var hits = BoltNetwork.RaycastAll(new Ray(pos, fireOriginForward), fireCommand.ServerFrame))
						{
							BoltLog.Warn("Hits: {0}", hits.count);

							for (int i = 0; i < hits.count; ++i)
							{
								var hit = hits.GetHit(i);
								var serializer = hit.body.GetComponent<MoveAndShoot_PlayerController>();

								if (serializer != null)
								{
									BoltLog.Warn("HIT: {0}", serializer.entity.NetworkId);

									HitHandler(serializer.entity, weapon.amount);
								}
							}
						}
					}
				}

				fireCommand.Result.Position = transform.localPosition;
				fireCommand.Result.Yaw = transform.localRotation.eulerAngles.y;
			}
		}

		#region Event Handlers

		private void OnFireDamageHandler()
		{
			if (weaponDamage.fx != null) { weaponDamage.fx.Play(); }
		}

		private void OnFireHeadHandler()
		{
			if (weaponHeal.fx != null) { weaponHeal.fx.Play(); }
		}

		private void HitHandler(BoltEntity entity, int weaponAmount)
		{
			if (entity.IsOwner)
			{
				BoltLog.Info("Hit {0} with {1}", entity.NetworkId, weaponAmount);

				// Get Player State
				var moveAndShootPlayerState = entity.GetState<IMoveAndShootPlayer>();

				// Get Current Health and Apply weapon change
				var targetHealth = moveAndShootPlayerState.Health + weaponAmount;

				// Clamp result value a put back on the state
				moveAndShootPlayerState.Health = Mathf.Clamp(targetHealth, 0, 100);
			}
		}

		#endregion

		#region Utils

		private void Move(Vector3 dir, float Yaw, out Vector3 resultPosition, out Vector3 resultVelocity,
			out float resultYaw)
		{
			if (_cc.isGrounded)
			{
				_localVelocity.y = 0;
			}

			// Move
			var motion = dir * moveSpeed * BoltNetwork.FrameDeltaTime;
			_cc.Move(motion);

			// Apply Gravity
			_localVelocity.y += _gravityValue * BoltNetwork.FrameDeltaTime;
			_cc.Move(_localVelocity * BoltNetwork.FrameDeltaTime);

			transform.localRotation = Quaternion.Euler(0, Yaw, 0);

			resultPosition = transform.localPosition;
			resultVelocity = _cc.velocity;
			resultYaw = transform.localRotation.eulerAngles.y;
		}

		private void SetState(Vector3 position, Vector3 velocity, float yaw)
		{
			_cc.enabled = false;
			transform.localPosition = position;
			_cc.enabled = true;

			transform.localRotation = Quaternion.Euler(0, yaw, 0);
		}

		private void PollInput()
		{
			var forward = Input.GetKey(KeyCode.W);
			var backward = Input.GetKey(KeyCode.S);
			var left = Input.GetKey(KeyCode.A);
			var right = Input.GetKey(KeyCode.D);

			if (forward ^ backward)
			{
				_input._dir.z = forward ? 1 : -1;
			}
			else
			{
				_input._dir.z = 0;
			}

			if (left ^ right)
			{
				_input._dir.x = right ? 1 : -1;
			}
			else
			{
				_input._dir.x = 0;
			}

			_input._dir.Normalize();

			var ray = _camera.ScreenPointToRay(Input.mousePosition);

			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100, groundMask))
			{
				var point = hitInfo.point;

				Debug.DrawRay(point, Vector3.up * 10, Color.yellow);

				var origin = transform.localPosition;
				origin.y = point.y;

				var dir = point - origin;

				_input._angle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);

				if (_input._angle < 0)
				{
					_input._angle = 360 + _input._angle;
				}
			}

			var button1 = Input.GetMouseButton(0);
			var button2 = Input.GetMouseButton(1);

			if (button1 ^ button2)
			{
				_input._fired = button1;
			}
		}

		#endregion
	}
}
