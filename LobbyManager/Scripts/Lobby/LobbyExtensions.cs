using System;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

namespace Bolt.Samples.Photon.Lobby
{
    public static class LobbyExtensions
    {
        public static LobbyPhotonPlayer GetLobbyPlayer(this BoltConnection connection)
        {
            if (connection == null)
            {
                return LobbyPlayerList._instance.ServerPlayer;
            }

            return (LobbyPhotonPlayer)connection.UserData;
        }

        public static bool CanJoin(this UdpSession session)
        {
            return session.ConnectionsCurrent != session.ConnectionsMax;
        }

        public static long Ping(this UdpSession session, Action callback)
        {
            callback();
            return 0;
        }
    }
}