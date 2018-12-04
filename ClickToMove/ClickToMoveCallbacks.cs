
[BoltGlobalBehaviour(BoltNetworkModes.Server, "ClickToMoveServerAuth")]
public class ClickToMoveCallbacks : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string scene)
    {
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.ClickToMovePlayer);
        player.TakeControl();
    }


    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.ClickToMovePlayer);
        player.AssignControl(connection);
    }
}
