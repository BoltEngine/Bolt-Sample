using System;
using Bolt.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bolt.Samples.HeadlessServer
{
    public class HeadlessServerManager : Bolt.GlobalEventListener
    {
        public string Map = "";
        public string GameType = "";
        public string RoomID = "";

        public override void BoltStartBegin()
        {
            // Register any Protocol Token that are you using
            BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
        }

        public override void BoltStartDone()
        {
            if (BoltNetwork.IsServer)
            {
                // Create some room custom properties
                PhotonRoomProperties roomProperties = new PhotonRoomProperties();

                roomProperties.AddRoomProperty("t", GameType); // ex: game type
                roomProperties.AddRoomProperty("m", Map); // ex: map id

                roomProperties.IsOpen = true;
                roomProperties.IsVisible = true;

                // If RoomID was not set, create a random one
                if (RoomID.Length == 0)
                {
                    RoomID = new Guid().ToString();
                }

                // Create the Photon Room
                BoltNetwork.SetServerInfo(RoomID, roomProperties);

                // Load the requested Level
                BoltNetwork.LoadScene(Map);
            }
        }

        // Use this for initialization
        void Start()
        {
            // Get custom arguments from command line
            Map = GetArg("-m", "-map") ?? Map;
            GameType = GetArg("-t", "-gameType") ?? GameType; // ex: get game type from command line
            RoomID = GetArg("-r", "-room") ?? RoomID;

            // Validate the requested Level
            var validMap = false;

            foreach (string value in BoltScenes.AllScenes)
            {
                if (SceneManager.GetActiveScene().name != value)
                {
                    if (Map == value)
                    {
                        validMap = true;
                        break;
                    }
                }
            }

            if (!validMap)
            {
                BoltLog.Error("Invalid configuration: please verify level name");
                Application.Quit();
            }

            // Start the Server
            BoltLauncher.StartServer();
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// Utility function to detect if the game instance was started in headless mode.
        /// </summary>
        /// <returns><c>true</c>, if headless mode was ised, <c>false</c> otherwise.</returns>
        public static bool IsHeadlessMode()
        {
            return Environment.CommandLine.Contains("-batchmode") && Environment.CommandLine.Contains("-nographics");
        }

        static string GetArg(params string[] names)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                foreach (var name in names)
                {
                    if (args[i] == name && args.Length > i + 1)
                    {
                        return args[i + 1];
                    }
                }
            }

            return null;
        }
    }
}