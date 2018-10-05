using UnityEngine;
using System.Collections;
using System.Text;
using Bolt.a2s;

namespace Bolt.AdvancedTutorial
{
    [BoltGlobalBehaviour("Level1")]
    public class PlayerCallbacks : GlobalEventListener
    {
        public override void SceneLoadLocalDone(string map)
        {
            // ui
            GameUI.Instantiate();

            // camera
            PlayerCamera.Instantiate();

            if (BoltNetwork.isServer)
            {
                A2SManager.SetPlayerInfo(null, "SERVER");

                A2SManager.UpdateServerInfo(
                    gameName: "Bolt Advanced Tutorial",
                    serverName: "Photon Bolt Server",
                    map: map,
                    version: "1.0",
                    serverType: ServerType.Listen,
                    visibility: Visibility.PUBLIC
                );
            }
        }

        public override void SceneLoadLocalBegin(string scene, Bolt.IProtocolToken token)
        {
            BoltLog.Info("SceneLoadLocalBegin-Token: {0}", token);
        }

        public override void SceneLoadLocalDone(string scene, Bolt.IProtocolToken token)
        {
            BoltLog.Info("SceneLoadLocalDone-Token: {0}", token);
        }

        public override void SceneLoadRemoteDone(BoltConnection connection, Bolt.IProtocolToken token)
        {
            BoltLog.Info("SceneLoadRemoteDone-Token: {0}", token);

            if (BoltNetwork.isServer)
            {
                A2SManager.SetPlayerInfo(connection, connection.ConnectionId.ToString());
            }
        }

        public override void ControlOfEntityGained(BoltEntity entity)
        {
            // add audio listener to our character
            entity.gameObject.AddComponent<AudioListener>();

            // set camera callbacks
            PlayerCamera.instance.getAiming = () => entity.GetState<IPlayerState>().Aiming;
            PlayerCamera.instance.getHealth = () => entity.GetState<IPlayerState>().health;
            PlayerCamera.instance.getPitch = () => entity.GetState<IPlayerState>().pitch;

            // set camera target
            PlayerCamera.instance.SetTarget(entity);
        }
    }

}
