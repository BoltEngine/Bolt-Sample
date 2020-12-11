using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Utils;

namespace Bolt.AdvancedTutorial
{
	public class PlayerController : EntityEventListener<IPlayerState>
	{
		const float MOUSE_SENSEITIVITY = 2f;

		bool forward;
		bool backward;
		bool left;
		bool right;
		bool jump;
		bool aiming;
		bool fire;

		int weapon;

		float yaw;
		float pitch;

		PlayerMotor _motor;

		[SerializeField]
		WeaponBase[] _weapons;

		[SerializeField]
		AudioSource _weaponSfxSource;

		public WeaponBase activeWeapon
		{
			get { return _weapons[state.weapon]; }
		}



		void Awake()
		{
			_motor = GetComponent<PlayerMotor>();
		}

		void Update()
		{
			PollKeys(true);

			if (entity.IsOwner && entity.HasControl && Input.GetKey(KeyCode.L))
			{
				for (int i = 0; i < 100; ++i)
				{
					BoltNetwork.Instantiate(BoltPrefabs.SceneCube, new Vector3(Random.value * 512, Random.value * 512, Random.value * 512), Quaternion.identity);
				}
			}
		}

		void PollKeys(bool mouse)
		{
			forward = Input.GetKey(KeyCode.W);
			backward = Input.GetKey(KeyCode.S);
			left = Input.GetKey(KeyCode.A);
			right = Input.GetKey(KeyCode.D);
			jump = Input.GetKey(KeyCode.Space);
			aiming = Input.GetMouseButton(1);
			fire = Input.GetMouseButton(0);

			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				weapon = 0;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				weapon = 1;
			}

			if (mouse)
			{
				yaw += (Input.GetAxisRaw("Mouse X") * MOUSE_SENSEITIVITY);
				yaw %= 360f;

				pitch += (-Input.GetAxisRaw("Mouse Y") * MOUSE_SENSEITIVITY);
				pitch = Mathf.Clamp(pitch, -85f, +85f);
			}
		}

		public override void Attached()
		{
			if (entity.IsOwner)
			{
				state.tokenTest = new TestToken() { Number = 1337 };
			}

			state.AddCallback("tokenTest", () =>
			{
				BoltLog.Info("Received token in .tokenTest property {0}", state.tokenTest);
			});

			state.SetTransforms(state.transform, transform);
			state.SetAnimator(GetComponentInChildren<Animator>());

			// setting layerweights 
			state.Animator.SetLayerWeight(0, 1);
			state.Animator.SetLayerWeight(1, 1);

			state.OnFire += OnFire;
			state.AddCallback("weapon", WeaponChanged);

			// setup weapon
			WeaponChanged();
		}

		void WeaponChanged()
		{
			// setup weapon
			for (int i = 0; i < _weapons.Length; ++i)
			{
				_weapons[i].gameObject.SetActive(false);
			}

			_weapons[state.weapon].gameObject.SetActive(true);
		}

		void OnFire()
		{
			// play sfx
			_weaponSfxSource.PlayOneShot(activeWeapon.fireSound);

			GameUI.instance.crosshair.Spread += 0.1f;

			// 
			activeWeapon.Fx(entity);
		}

		public void ApplyDamage(byte damage)
		{
			if (!state.Dead)
			{

				state.health -= damage;

				if (state.health > 100 || state.health < 0)
				{
					state.health = 0;
				}
			}

			if (state.health == 0)
			{
				entity.Controller.GetPlayer().Kill();
			}
		}

		public override void SimulateOwner()
		{
			if ((BoltNetwork.Frame % 5) == 0 && (state.Dead == false))
			{
				state.health = (byte)Mathf.Clamp(state.health + 1, 0, 100);
			}
		}

		public override void SimulateController()
		{
			PollKeys(false);

			IPlayerCommandInput input = PlayerCommand.Create();

			input.forward = forward;
			input.backward = backward;
			input.left = left;
			input.right = right;
			input.jump = jump;

			input.aiming = aiming;
			input.fire = fire;

			input.yaw = yaw;
			input.pitch = pitch;

			input.weapon = weapon;
			input.Token = new TestToken();

			entity.QueueInput(input);
		}

		public override void ExecuteCommand(Command c, bool resetState)
		{
			if (state.Dead)
			{
				return;
			}

			PlayerCommand cmd = (PlayerCommand)c;

			if (resetState)
			{
				_motor.SetState(cmd.Result.position, cmd.Result.velocity, cmd.Result.isGrounded, cmd.Result.jumpFrames);
			}
			else
			{
				// move and save the resulting state
				var result = _motor.Move(cmd.Input.forward, cmd.Input.backward, cmd.Input.left, cmd.Input.right, cmd.Input.jump, cmd.Input.yaw);

				cmd.Result.position = result.position;
				cmd.Result.velocity = result.velocity;
				cmd.Result.jumpFrames = result.jumpFrames;
				cmd.Result.isGrounded = result.isGrounded;

				if (cmd.IsFirstExecution)
				{
					// animation
					AnimatePlayer(cmd);

					// set state pitch
					state.pitch = cmd.Input.pitch;
					state.weapon = cmd.Input.weapon;
					state.Aiming = cmd.Input.aiming;

					// deal with weapons
					if (cmd.Input.aiming && cmd.Input.fire)
					{
						FireWeapon(cmd);
					}
				}

				if (entity.IsOwner)
				{
					cmd.Result.Token = new TestToken();
				}
			}
		}

		void AnimatePlayer(PlayerCommand cmd)
		{
			// FWD <> BWD movement
			if (cmd.Input.forward ^ cmd.Input.backward)
			{
				state.MoveZ = cmd.Input.forward ? 1 : -1;
			}
			else
			{
				state.MoveZ = 0;
			}

			// LEFT <> RIGHT movement
			if (cmd.Input.left ^ cmd.Input.right)
			{
				state.MoveX = cmd.Input.right ? 1 : -1;
			}
			else
			{
				state.MoveX = 0;
			}

			// JUMP
			if (_motor.jumpStartedThisFrame)
			{
				state.Jump();
			}
		}

		void FireWeapon(PlayerCommand cmd)
		{
			if (activeWeapon.fireFrame + activeWeapon.refireRate <= BoltNetwork.ServerFrame)
			{
				activeWeapon.fireFrame = BoltNetwork.ServerFrame;

				state.Fire();

				// if we are the owner and the active weapon is a hitscan weapon, do logic
				if (entity.IsOwner)
				{
					activeWeapon.OnOwner(cmd, entity);
				}
			}
		}
	}
}
