using System;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

namespace Bolt.Samples.GettingStarted
{
    public class Menu : Bolt.GlobalEventListener
    {
        bool ShowGui = true;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            BoltLauncher.SetUdpPlatform(new PhotonPlatform());
        }

        void OnGUI()
        {
            if (!ShowGui) { return; }

            GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

            if (GUILayout.Button("Start Server", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                // START SERVER
                BoltLauncher.StartServer();
            }

            if (GUILayout.Button("Start Client", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                // START CLIENT
                BoltLauncher.StartClient();
            }

            GUILayout.EndArea();
        }

        public override void BoltStartBegin()
        {
            ShowGui = false;
        }

        public override void BoltStartDone()
        {
            if (BoltNetwork.IsServer)
            {
                string matchName = Guid.NewGuid().ToString();

                BoltNetwork.SetServerInfo(matchName, null);
                BoltNetwork.LoadScene("Tutorial1");
            }
        }

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

            foreach (var session in sessionList)
            {
                UdpSession photonSession = session.Value as UdpSession;

                if (photonSession.Source == UdpSessionSource.Photon)
                {
                    BoltNetwork.Connect(photonSession);
                }
            }
        }
    }
}