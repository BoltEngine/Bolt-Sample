using UnityEngine;
using System.Collections;

namespace Bolt.Samples.AdvancedTutorial
{
    public class GameSpawnTimer : Bolt.GlobalEventListener
    {
        BoltEntity me;
        IPlayerState meState;

        [SerializeField]
        TypogenicText timer;

        public override void ControlOfEntityGained(BoltEntity entity)
        {
            if (entity.GetComponent<PlayerController>())
            {
                me = entity;
                meState = me.GetState<IPlayerState>();
            }
        }

        public override void ControlOfEntityLost(BoltEntity entity)
        {
            if (entity.GetComponent<PlayerController>())
            {
                me = null;
                meState = null;
            }
        }

        void Update()
        {
            // lock in middle of screen
            transform.position = Vector3.zero;

            // update timer
            if (me && meState != null)
            {
                if (meState.Dead)
                {
                    timer.Set(Mathf.Max(0, (meState.respawnFrame - BoltNetwork.Frame) / BoltNetwork.FramesPerSecond).ToString());
                }
                else
                {
                    timer.Set("");
                }
            }
            else
            {
                timer.Set("");
            }
        }
    }
}