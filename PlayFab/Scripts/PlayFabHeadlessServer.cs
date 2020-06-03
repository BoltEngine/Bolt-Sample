using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bolt.Samples.PlayFab
{
	/// <summary>
	/// This class is responsible for running the Bolt Server based on the configurations found in the PlayFab
	/// Thunderhead configuration.
	/// 
	/// Use this component in conjunction with the <see cref="PlayFab.PlayFabMultiplayerAgentView"/> in order to
	/// get your instance running properly.
	///
	/// For more information, look the Photon Bolt PlayFab integration tutorial on the
	/// Docs Page <see cref="https://doc.photonengine.com/en-us/bolt/current/getting-started/overview"/>
	/// </summary>
	public partial class PlayFabHeadlessServer : Bolt.GlobalEventListener
	{
		/// <summary>
		/// Game Scene to be loaded by the Server
		/// </summary>
		[SerializeField] private string GameScene;

		/// <summary>
		/// Flag to signal PlayFabMultiplayerAgentAPI to show debug info
		/// </summary>
		[SerializeField] private bool Debugging = true;

		private HeadlessServerConfig config;

		private void Start()
		{
			if (BuildConfig() && IsHeadlessMode())
			{
				Application.targetFrameRate = 60;
				DontDestroyOnLoad(this.gameObject);

				// Notify PlayFab Agent that we are ready
				PlayFabStart();

				// Run Bolt Server
				OnServerActive();
			}
			else
			{
				// Load Client Menu Scene
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			}
		}

		/// <summary>
		/// Build the Server Configuration reading the command line arguments
		/// </summary>
		/// <returns>True if the configuration is set, false otherwise</returns>
		private bool BuildConfig()
		{
			config = new HeadlessServerConfig()
			{
				Map = GetArg("-m", "-map") ?? GameScene,
			};

			return string.IsNullOrEmpty(config.Map) == false && BoltScenes.AllScenes.Contains(config.Map);
		}

		/// <summary>
		/// Check if the executable is running in Headless mode
		/// </summary>
		/// <returns>True if using headless mode arguments, false otherwise</returns>
		private bool IsHeadlessMode()
		{
			return Environment.CommandLine.Contains("-batchmode") && Environment.CommandLine.Contains("-nographics");
		}

		/// <summary>
		/// Get argument values by name from the command line list of arguments passed when running the application
		/// Search for arguments like "-arg value", you pass "-arg" and it returns "value"
		/// </summary>
		/// <param name="names">List of arguments to search for</param>
		/// <returns>String value of the argument</returns>
		private string GetArg(params string[] names)
		{
			var args = Environment.GetCommandLineArgs();
			for (int i = 0; i < args.Length; i++)
			{
				foreach (var arg in names)
				{
					if (args[i] == arg && args.Length > i + 1)
					{
						return args[i + 1];
					}
				}
			}

			return null;
		}

#if !ENABLE_PLAYFABSERVER_API
		private void PlayFabStart() { }
		private void OnServerActive() { }
#endif
	}
}