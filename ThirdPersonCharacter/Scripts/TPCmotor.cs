using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Bolt;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TPCmotor : MonoBehaviour
{
	public struct State
	{
		public Vector3 position;
		public Vector3 velocity;
		public bool isGrounded;
		public int jumpFrames;
	}

	State _state;
	CharacterController _cc;

	[SerializeField]
	float skinWidth = 0.08f;

	[SerializeField]
	float gravityForce = -9.81f;

	[SerializeField]
	float jumpForce = +20f;

	[SerializeField]
	int jumpTotalFrames = 30;

	[SerializeField]
	float movingSpeed = 100f;

	[SerializeField]
	float maxVelocity = 32f;

	[SerializeField]
	Vector3 drag = new Vector3(1f, 0f, 1f);

	[SerializeField]
	LayerMask layerMask;
#pragma warning disable 0649
	Vector3 m_GroundNormal;
#pragma warning restore 0649
	Vector3 sphere
	{
		get
		{
			Vector3 p;

			p = transform.position;
			p.y += _cc.radius;
			p.y -= (skinWidth * 2);

			return p;
		}
	}

	Vector3 waist
	{
		get
		{
			Vector3 p;

			p = transform.position;
			p.y += _cc.height / 2f;

			return p;
		}
	}

	public bool jumpStartedThisFrame
	{
		get
		{
			return _state.jumpFrames == (jumpTotalFrames - 1);
		}
	}

	void Awake()
	{
		_cc = GetComponent<CharacterController>();
		_state = new State();
		_state.position = transform.localPosition;
	}

	public void SetState(Vector3 position, Vector3 velocity, bool isGrounded, int jumpFrames)
	{
		// assign new state
		_state.position = position;
		_state.velocity = velocity;
		_state.jumpFrames = jumpFrames;
		_state.isGrounded = isGrounded;

		// assign local position
		transform.localPosition = _state.position;
	}

	void Move(Vector3 velocity)
	{
		bool isGrounded = false;

		isGrounded = isGrounded || _cc.Move(velocity * BoltNetwork.FrameDeltaTime) == CollisionFlags.Below;
		isGrounded = isGrounded || _cc.isGrounded;
		isGrounded = isGrounded || Physics.CheckSphere(sphere, _cc.radius, layerMask);

		if (isGrounded && !_state.isGrounded)
		{
			_state.velocity = new Vector3();
		}

		_state.isGrounded = isGrounded;
		_state.position = transform.localPosition;
	}

	public State Move(bool crouch, float Forward, bool jump, Vector3 move, float Turn)
	{
		var moving = false;
		var movingDir = Vector3.zero;

		moving = true;
		movingDir = move;

		if (movingDir.magnitude > 1f)
			movingDir.Normalize();
		movingDir = transform.InverseTransformDirection(move);

		movingDir = Vector3.ProjectOnPlane(movingDir, m_GroundNormal);

		/*
		if (forward ^ backward)
		{
		    movingDir.z = forward ? +1 : -1;
		}

		if (left ^ right)
		{
		    movingDir.x = right ? +1 : -1;
		}

		if (movingDir.x != 0 || movingDir.z != 0)
		{
		    moving = true;
		    movingDir = Vector3.Normalize(Quaternion.Euler(0, yaw, 0) * movingDir);
		}
		*/

		//
		if (_state.isGrounded)
		{
			if (jump && _state.jumpFrames == 0)
			{
				_state.jumpFrames = (byte) jumpTotalFrames;
				_state.velocity += move * movingSpeed;
			}

			if (moving && _state.jumpFrames == 0)
			{
				Move(move * movingSpeed);
			}
		}
		else
		{
			_state.velocity.y += gravityForce * BoltNetwork.FrameDeltaTime;
		}

		if (_state.jumpFrames > 0)
		{
			// calculate force
			float force;
			force = (float) _state.jumpFrames / (float) jumpTotalFrames;
			force = jumpForce * force;

			Move(new Vector3(0, force, 0));
		}

		// decrease jump frames
		_state.jumpFrames = Mathf.Max(0, _state.jumpFrames - 1);

		// clamp velocity
		_state.velocity = Vector3.ClampMagnitude(_state.velocity, maxVelocity);

		// apply drag
		_state.velocity.x = ApplyDrag(_state.velocity.x, drag.x);
		_state.velocity.y = ApplyDrag(_state.velocity.y, drag.y);
		_state.velocity.z = ApplyDrag(_state.velocity.z, drag.z);

		// this might seem weird, but it actually gets around a ton of issues - we basically apply 
		// gravity on the Y axis on every frame to simulate instant gravity if you step over a ledge
		_state.velocity.y = Mathf.Min(_state.velocity.y, gravityForce);

		// apply movement
		Move(_state.velocity);

		// set local rotation
		transform.Rotate(0, Mathf.Atan2(movingDir.x, movingDir.z) * Mathf.Lerp(180, 360, movingDir.z) * Time.deltaTime, 0);

		//transform.localRotation = Quaternion.Euler(0, yaw, 0);

		// detect tunneling
		DetectTunneling();

		// update position
		_state.position = transform.localPosition;

		// done
		return _state;
	}

	float ApplyDrag(float value, float drag)
	{
		if (value < 0)
		{
			return Mathf.Min(value + (drag * BoltNetwork.FrameDeltaTime), 0f);
		}

		else if (value > 0)
		{
			return Mathf.Max(value - (drag * BoltNetwork.FrameDeltaTime), 0f);
		}

		return value;
	}

	void DetectTunneling()
	{
		RaycastHit hit;

		if (Physics.Raycast(waist, Vector3.down, out hit, _cc.height / 2, layerMask))
		{
			transform.position = hit.point;
		}
	}

	void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = _state.isGrounded ? Color.green : Color.red;
			Gizmos.DrawWireSphere(sphere, _cc.radius);

			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(waist, waist + new Vector3(0, -(_cc.height / 2f), 0));
		}
	}
}
