using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "serverAuthTPC")]
public class ThirdPersonControllerCallbacks : Bolt.GlobalEventListener
{

    public override void SceneLoadLocalDone(string map)
    {
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.ThirdPersonControllerServerAuth);
        player.TakeControl();
    }


    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.ThirdPersonControllerServerAuth);
        player.AssignControl(connection);
    }


}
