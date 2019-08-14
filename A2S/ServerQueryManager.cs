using Bolt.a2s;

[BoltGlobalBehaviour(BoltNetworkModes.Server)]
public class ServerQueryManager : Bolt.GlobalEventListener
{
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public override void BoltStartDone()
	{
		A2SManager.SetServerRules("rule1", "value1");
		A2SManager.SetServerRules("rule2", "value2");
	}

	public override void SceneLoadLocalDone(string map)
	{
		A2SManager.SetPlayerInfo(null, "Photon Server");

		A2SManager.UpdateServerInfo(
			gameName: "Bolt Simple Tutorial",
			serverName: "Photon Bolt Server",
			map : map,
			version: "1.0",
			serverType : ServerType.Listen,
			visibility : Visibility.PUBLIC
		);
	}

	public override void SceneLoadRemoteDone(BoltConnection connection)
	{
		A2SManager.SetPlayerInfo(connection, "Conn: " + connection.ConnectionId.ToString(), 0);
	}
}