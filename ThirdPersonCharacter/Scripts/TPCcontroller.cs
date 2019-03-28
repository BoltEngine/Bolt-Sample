using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class TPCcontroller : Bolt.EntityEventListener<ITPCstate>
{
#pragma warning disable 0649
	TPCmotor _motor;
	private Transform m_Cam;
	Animator m_Animator;
	CharacterController _cc;
	private Vector3 m_CamForward;
	float m_ForwardAmount;

	float m_TurnAmount;
	bool m_IsGrounded;
	Vector3 m_GroundNormal;
	Vector3 _velocity;

	public LayerMask layerMask;
	float skinWidth = 0.08f;
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

	public override void Attached()
	{
		_motor = GetComponent<TPCmotor>();
		/// Debug.Log(entity);
		// Debug.Log(entity.GetInstanceID());

		state.SetTransforms(state.transform, transform);
		_cc = GetComponent<CharacterController>();
		m_Animator = GetComponent<Animator>();

		state.AddCallback("forward", () => { m_Animator.SetFloat("Forward", state.forward); });
		state.AddCallback("turn", () => { m_Animator.SetFloat("Turn", state.turn); });
		state.AddCallback("crouch", () => { m_Animator.SetBool("Crouch", state.crouch); });
	}

	// Use this for initialization
	void Start()
	{

	}

	void FixedUpdate()
	{
		if (entity.IsControllerOrOwner)
		{
			//  if (_velocity != null)
			//     _cc.Move(_velocity);

		}
	}

	// Update is called once per frame
	public override void SimulateController()
	{
		ITPCCommandInput input = TPCCommand.Create();

		float h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");

		if (m_Cam == null && GameObject.Find("Cameras"))
		{
			Transform cam = GameObject.Find("Cameras").transform.GetChild(0).GetChild(0).GetChild(0);
			m_Cam = cam;

		}

		Vector3 move = Vector3.zero;

		if (m_Cam != null)
		{
			// calculate camera relative direction to move:
			m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
			//input.camPosition = v * m_CamForward + h * m_Cam.right;

			move = v * m_CamForward + h * m_Cam.right;
			if (move.magnitude > 1f)
				move.Normalize();
			move = transform.InverseTransformDirection(move);

			move = Vector3.ProjectOnPlane(move, m_GroundNormal);

			m_ForwardAmount = move.z;
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			// m_Crouching = Input.GetKey(KeyCode.C);

			input.forward = m_ForwardAmount;
			input.turn = m_TurnAmount;
			// m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			// m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
		}

		//_cc.Move(v * m_CamForward / 20 + h * m_Cam.right * Time.deltaTime * 2); //time.detlatime for both

		input.move = 3 * (v * m_CamForward * Time.deltaTime + h * m_Cam.right * Time.deltaTime);
		input.crouch = Input.GetKey(KeyCode.C);
		input.jump = Input.GetKey(KeyCode.Space);
		entity.QueueInput(input);

	}

	public override void ExecuteCommand(Bolt.Command c, bool resetState)
	{
		TPCCommand cmd = (TPCCommand) c;

		if (resetState)
		{
			_motor.SetState(cmd.Result.position, cmd.Result.velocity, cmd.Result.isGrounded, cmd.Result.jumpFrames);

			//transform.localPosition = cmd.Result.position;
			//_cc.Move(cmd.Result.velocity);
			//_velocity = cmd.Input.move;
			//_motor.SetState(cmd.Result.position, cmd.Result.velocity, cmd.Result.isGrounded, cmd.Result.jumpFrames);
		}
		else
		{
			var result = _motor.Move(cmd.Input.crouch, cmd.Input.forward, cmd.Input.jump, cmd.Input.move, cmd.Input.turn);

			cmd.Result.position = result.position;
			cmd.Result.velocity = result.velocity;
			cmd.Result.jumpFrames = result.jumpFrames;
			cmd.Result.isGrounded = result.isGrounded;

			m_Animator.SetFloat("Forward", cmd.Input.forward, 0f, Time.deltaTime);
			m_Animator.SetFloat("Turn", cmd.Input.turn, 0f, Time.deltaTime);
			m_Animator.SetBool("Crouch", cmd.Input.crouch);
			m_Animator.SetBool("OnGround", cmd.Result.isGrounded);

			if (entity.IsOwner)
			{
				state.forward = m_Animator.GetFloat("Forward");
				state.turn = m_Animator.GetFloat("Turn");
				state.crouch = m_Animator.GetBool("Crouch");
				state.grounded = m_Animator.GetBool("OnGround");
			}

			/*

            Vector3 move = cmd.Input.move;
            if (move.magnitude > 1f)
                move.Normalize();
            move = transform.InverseTransformDirection(move);

            move = Vector3.ProjectOnPlane(move, m_GroundNormal);

            m_TurnAmount = Mathf.Atan2(move.x, move.z);
            m_ForwardAmount = move.z;

            m_IsGrounded = _cc.Move(cmd.Input.move * BoltNetwork.frameDeltaTime) == CollisionFlags.Below;
            m_IsGrounded = m_IsGrounded || _cc.isGrounded;
            m_IsGrounded = m_IsGrounded || Physics.CheckSphere(sphere, _cc.radius, layerMask);
            Debug.Log(m_IsGrounded);

            Vector3 _vel = cmd.Input.move;
            if (m_IsGrounded == false)
                _vel.y -= 0.1f;
            else if(cmd.Input.jump == true)
                _vel.y += 100f;

            _cc.Move(_vel);

            //_velocity = cmd.Input.move;
            //_cc.Move(cmd.Input.move);

            float turnSpeed = Mathf.Lerp(180, 360, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);

            m_Animator.SetFloat("Forward", cmd.Input.Forward, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Turn", cmd.Input.Turn, 0.1f, Time.deltaTime);
            m_Animator.SetBool("Crouch", cmd.Input.crouch);

            if (entity.isOwner)
            {
                state.Forward = m_Animator.GetFloat("Forward");
                state.Turn = m_Animator.GetFloat("Turn");
                state.Crouch = m_Animator.GetBool("Crouch");
            }

            cmd.Result.position = transform.localPosition;
            cmd.Result.velocity = (_cc.velocity);

    */
		}
	}

}