using UnityEngine;
using System.Collections;
using Bolt.Samples.Photon.Lobby;

[BoltGlobalBehaviour("PhotonGame")]
public class PhotonLobbyInGame : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string map)
    {
        BomberPlayerController.Spawn();
    }

    //public override void SceneLoadRemoteDone(BoltConnection connection)
    //{
    //    //BomberPlayerController.Spawn();
    //    //BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.BomberPlayer);
    //    //entity.AssignControl(connection);
    //}
}
