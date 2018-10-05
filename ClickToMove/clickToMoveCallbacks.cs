
[BoltGlobalBehaviour(BoltNetworkModes.Server, "clickToMoveServerAuth")]
public class ClickToMoveCallbacks : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string map)
    {
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.clickToMovePlayer);
        player.TakeControl();
    }


    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.clickToMovePlayer);
        player.AssignControl(connection);
    }
}
