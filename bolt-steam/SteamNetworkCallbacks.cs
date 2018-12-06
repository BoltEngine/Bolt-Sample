using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bolt;
using Steamworks;
using UnityEngine;

static class LocalData
{
    public static string ServerScene = "Level1";
}

public class SteamNetworkCallbacks : Bolt.GlobalEventListener
{
    public static bool ListenServer = true;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public override void Connected(BoltConnection connection)
    {
        if (SteamHub.LobbyActive != null && SteamManager.Initialized)
        {
            var token = (SteamToken)connection.ConnectToken;
            var activeLobby = SteamHub.LobbyActive;
            bool found = false;

            foreach (var m in activeLobby.AllMembers)
            {
                if (m.m_SteamID == token.SteamID)
                {
                    connection.UserData = "CLIENT:" + SteamFriends.GetFriendPersonaName(m) + " " + connection.RemoteEndPoint.Port;
                    Debug.Log(connection.UserData);
                    found = true;
                    break;
                }
            }

            if (found == false)
            {
                connection.Disconnect();
            }
        }
    }

    public override void BoltStartDone()
    {
        if (SteamHub.LobbyActive != null && SteamManager.Initialized)
        {
            BoltNetwork.RegisterTokenClass<SteamToken>();

            Debug.Log("enteredGame");
            string enterMessage = "enteredGame" + SteamUser.GetSteamID();
            byte[] enterMsgAsBytes = Encoding.ASCII.GetBytes(enterMessage);
            SteamMatchmaking.SendLobbyChatMsg(SteamHub.LobbyActive.LobbyId, enterMsgAsBytes, enterMsgAsBytes.Length + 1);

            if (GameObject.Find("Main Camera").GetComponent<SteamLobby>().isOwner())
            {
                BoltNetwork.LoadScene(LocalData.ServerScene);
            }
            else
            {
#if !BOLT_CLOUD
                var token = new SteamToken();
                Debug.Log(SteamUser.GetSteamID().m_SteamID);
                token.SteamID = SteamUser.GetSteamID().m_SteamID;
                CSteamID serverID = GameObject.Find("Main Camera").GetComponent<SteamLobby>().getGameServerID();
                BoltNetwork.Connect(serverID.ToEndPoint(), token);
#else
                Debug.LogError("This call is only valid on Bolt Server version");
#endif
            }
        }
    }
}

