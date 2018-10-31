using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UdpKit;
using UnityEngine.SceneManagement;

namespace Bolt.Samples.AdvancedTutorial
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server, "Level1")]
    public class ServerCallbacks : Bolt.GlobalEventListener
    {
        public static bool ListenServer = true;

		public override bool PersistBetweenStartupAndShutdown()
		{
            return base.PersistBetweenStartupAndShutdown();
		}

		void Awake()
        {
            if (ListenServer)
            {
                Player.CreateServerPlayer();
                Player.serverPlayer.name = "SERVER";
            }
        }

        void FixedUpdate()
        {
            foreach (Player p in Player.allPlayers)
            {
                // if we have an entity, it's dead but our spawn frame has passed
                if (p.entity && p.state.Dead && p.state.respawnFrame <= BoltNetwork.ServerFrame)
                {
                    p.Spawn();
                }
            }
        }

        public override void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token)
        {
            BoltLog.Info("ConnectRequest", Color.red);

            if (token != null)
            {
                BoltLog.Info("Token Received", Color.red);
            }

            BoltNetwork.Accept(endpoint);
        }

        public override void ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token)
        {
            BoltLog.Info("ConnectAttempt", Color.red);
            base.ConnectAttempt(endpoint, token);
        }

        public override void Disconnected(BoltConnection connection)
        {
            BoltLog.Info("Disconnected", Color.red);
            base.Disconnected(connection);
        }

        public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
        {
            BoltLog.Info("ConnectRefused", Color.red);
            base.ConnectRefused(endpoint, token);
        }

        public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
        {
            BoltLog.Info("ConnectFailed", Color.red);
            base.ConnectFailed(endpoint, token);
        }

        public override void Connected(BoltConnection c)
        {
            BoltLog.Info("Connected", Color.red);

            c.UserData = new Player();
            c.GetPlayer().connection = c;
            c.GetPlayer().name = "CLIENT:" + c.RemoteEndPoint.Port;

            c.SetStreamBandwidth(1024 * 1024);
        }

        public override void BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            BoltLog.Warn("Bolt is shutting down");

            registerDoneCallback(() =>
            {
                BoltLog.Warn("Bolt is down");
                SceneManager.LoadScene(0);
            });
        }

        public override void SceneLoadRemoteDone(BoltConnection connection)
        {
            connection.GetPlayer().InstantiateEntity();
        }

        public override void SceneLoadLocalDone(string map)
        {
            if (Player.serverIsPlaying)
            {
                Player.serverPlayer.InstantiateEntity();
            }
        }

        public override void SceneLoadLocalBegin(string map)
        {
            foreach (Player p in Player.allPlayers)
            {
                p.entity = null;
            }
        }
    }
}
