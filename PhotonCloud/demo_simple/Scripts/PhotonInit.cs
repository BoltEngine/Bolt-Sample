using UnityEngine;
using System.Collections;
using System;
using UdpKit;
using Bolt;
using udpkit.platform.photon.photon;
using System.Collections.Generic;
using Bolt.photon;

namespace Bolt.Samples.Photon.Simple
{
    public class PhotonInit : Bolt.GlobalEventListener
    {
        // helper enum and attribute to hold which mode application is running on
        enum State
        {
            SelectMode,
            ModeServer,
            ModeClient
        }

        State _state;
        Dictionary<Guid, UdpSession> internalSessionList;

        public override void BoltStartBegin()
        {
            BoltNetwork.RegisterTokenClass<RoomProtocolToken>();
            BoltNetwork.RegisterTokenClass<ServerAcceptToken>();
            BoltNetwork.RegisterTokenClass<ServerConnectToken>();

            // PhotonRoomProperties is used to pass custom properties into your room
            BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
        }

        void Awake()
        {
            internalSessionList = new Dictionary<Guid, UdpSession>();

            // Optionally, you may want to config the Photon transport layer programatically:
            //BoltLauncher.SetUdpPlatform(new PhotonPlatform(new PhotonPlatformConfig
            //{
            //    AppId = "<your-app-id>",
            //    RegionMaster = "<your-region>",
            //    UsePunchThrough = true, // set to false, to disable PunchThrough
            //    MaxConnections = 32
            //}));
        }

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            BoltLog.Info("Session list updated: {0} total sessions", sessionList.Count);

            foreach (var session in sessionList)
            {
                UdpSession udpSession = session.Value as UdpSession;
                BoltLog.Info("UdpSession {0} Source: {1}", udpSession.HostName, udpSession.Source);

                if (udpSession.Source == UdpSessionSource.Photon)
                {
                    internalSessionList[udpSession.Id] = udpSession;
                }
            }
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

            switch (_state)
            {
                // starting Bolt is the same regardless of the transport layer
                case State.SelectMode:
                    if (GUILayout.Button("Start Server", GUILayout.ExpandWidth(true)))
                    {
                        BoltLauncher.StartServer();
                        _state = State.ModeServer;
                    }

                    if (GUILayout.Button("Start Client", GUILayout.ExpandWidth(true)))
                    {
                        BoltLauncher.StartClient();
                        _state = State.ModeClient;
                    }

                    break;

                // Publishing a session into the matchmaking server
                case State.ModeServer:
                    if (BoltNetwork.isRunning && BoltNetwork.isServer)
                    {
                        if (GUILayout.Button("Publish HostInfo And Load Map", GUILayout.ExpandWidth(true)))
                        {
                            // Normal Token
                            //RoomProtocolToken token = new RoomProtocolToken()
                            //{
                            //    ArbitraryData = "My DATA",
                            //    password = "mysuperpass123"
                            //};

                            // Use to pass custom properties to your room
                            PhotonRoomProperties roomProperties = new PhotonRoomProperties();

                            roomProperties.AddRoomProperty("t", 1); // ex: game type
                            roomProperties.AddRoomProperty("m", 4); // ex: map id

                            roomProperties.IsOpen = true;
                            roomProperties.IsVisible = true;

                            string matchName = "MyPhotonGame #" + UnityEngine.Random.Range(1, 100);

                            BoltNetwork.SetServerInfo(matchName, roomProperties);
                            BoltNetwork.LoadScene("Level1");
                        }
                    }
                    break;

                // for the client, after Bolt is innitialized, we should see the list
                // of available sessions and join one of them
                case State.ModeClient:

                    if (BoltNetwork.isRunning && BoltNetwork.isClient)
                    {
                        GUILayout.Label("Session List");
                        ShowSessionList(internalSessionList);
                    }
                    break;
            }

            GUILayout.EndArea();
        }

        void ShowSessionList(Dictionary<Guid, UdpSession> sessionList)
        {
            foreach (var session in sessionList)
            {
                UdpSession udpSession = session.Value as UdpSession;

                // Skip if is not a Photon session
                if (udpSession.Source != UdpSessionSource.Photon)
                {
                    BoltLog.Info("UdpSession with different Source: {0}", udpSession.Source);
                    continue;
                }

                PhotonSession photonSession = udpSession as PhotonSession;

                if (photonSession == null)
                {
                    continue;
                }

                string sessionDescription = String.Format("{0} ({1})",
                                  photonSession.HostName, photonSession.Id);

                IProtocolToken token = photonSession.GetProtocolToken();

                // Normal Token
                RoomProtocolToken roomToken = token as RoomProtocolToken;

                if (roomToken != null)
                {
                    sessionDescription += String.Format(" :: {0}", roomToken.ArbitraryData);
                }

                object prop_type = -1;
                object prop_map = -1;

                if (photonSession.Properties.ContainsKey("t"))
                {
                    prop_type = photonSession.Properties["t"];
                }

                if (photonSession.Properties.ContainsKey("m"))
                {
                    prop_map = photonSession.Properties["m"];
                }

                sessionDescription += String.Format(" :: {0} / {1}", prop_type, prop_map);

                if (GUILayout.Button(sessionDescription, GUILayout.ExpandWidth(true)))
                {
                    ServerConnectToken connectToken = new ServerConnectToken
                    {
                        data = "ConnectTokenData"
                    };

                    BoltNetwork.Connect(photonSession, connectToken);
                }
            }
        }
    }
}
