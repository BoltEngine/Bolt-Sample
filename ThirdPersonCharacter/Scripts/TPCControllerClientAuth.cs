using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Bolt.Samples.ThirdPerson
{
    public class TPCControllerClientAuth : Bolt.EntityEventListener<ITPCstate>
    {
        Animator anim;

        public override void Attached()
        {
            BoltLog.Info(transform.position);

            state.SetTransforms(state.transform, transform);

            anim = GetComponent<Animator>();

            if (!entity.isOwner)
            {
                Destroy(GetComponent<ThirdPersonUserControl>());
                Destroy(GetComponent<ThirdPersonCharacter>());
                Destroy(GetComponent<Rigidbody>());
                state.AddCallback("grounded", () => { anim.SetBool("OnGround", state.grounded); });
                state.AddCallback("crouch", () => { anim.SetBool("Crouch", state.crouch); });
                state.AddCallback("forward", () => { anim.SetFloat("Forward", state.forward); });
                state.AddCallback("turn", () => { anim.SetFloat("Turn", state.turn); });
                state.AddCallback("jump", () => { anim.SetFloat("Jump", state.jump); });
                state.AddCallback("jumpleg", () => { anim.SetFloat("JumpLeg", state.jumpleg); });
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                BoltLog.Info("q");
                state.SetTransforms(state.transform, null);
            }
        }

        void FixedUpdate()
        {
            if (entity.isOwner)
            {
                state.grounded = anim.GetBool("OnGround");
                state.crouch = anim.GetBool("Crouch");
                state.forward = anim.GetFloat("Forward");
                state.turn = anim.GetFloat("Turn");
                state.jump = anim.GetFloat("Jump");
                state.jumpleg = anim.GetFloat("JumpLeg");
            }
        }
    }
}