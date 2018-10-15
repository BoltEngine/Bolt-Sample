using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bolt;
using Steamworks;
using UnityEngine;

namespace Bolt.Samples.Steam
{
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
                        BoltLog.Info(connection.UserData);
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
#if !BOLT_CLOUD
        if (SteamHub.LobbyActive != null && SteamManager.Initialized)
        {
            BoltNetwork.RegisterTokenClass<SteamToken>();

            BoltLog.Info("enteredGame");
            string enterMessage = "enteredGame" + SteamUser.GetSteamID();
            byte[] enterMsgAsBytes = Encoding.ASCII.GetBytes(enterMessage);
            SteamMatchmaking.SendLobbyChatMsg(SteamHub.LobbyActive.LobbyId, enterMsgAsBytes, enterMsgAsBytes.Length + 1);

            if (GameObject.Find("Main Camera").GetComponent<SteamLobby>().isOwner())
            {
                BoltNetwork.LoadScene(LocalData.ServerScene);
            }
            else
            {
                var token = new SteamToken();
                BoltLog.Info(SteamUser.GetSteamID().m_SteamID);
                token.SteamID = SteamUser.GetSteamID().m_SteamID;
                CSteamID serverID = GameObject.Find("Main Camera").GetComponent<SteamLobby>().getGameServerID();
                BoltNetwork.Connect(serverID.ToEndPoint(), token);
            }
        }
#endif
        }
    }
}