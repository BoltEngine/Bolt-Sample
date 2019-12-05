using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Bolt.Samples.PlayFab
{
	public partial class PlayFabHeadlessServer : Bolt.GlobalEventListener
	{
		[SerializeField]
		private string GameScene;

		[SerializeField]
		private bool Debugging = true;

		[SerializeField]
		private bool AutoRunServer = false;

		private HeadlessServerConfig config;

		private void Start()
		{
			if (IsHeadlessMode() && BuildConfig())
			{
				Application.targetFrameRate = 60;
				DontDestroyOnLoad(this.gameObject);

				// Notify PlayFab Agent that we are ready
				PlayFabStart();

				if (config.AutoRun)
				{
					OnServerActive();
				}
			}
			else
			{
				Debug.LogError(MessageInvalidArguments);
				StartCoroutine(ShutdownRoutine());
			}
		}

		private bool BuildConfig()
		{
			config = new HeadlessServerConfig()
			{
				Map = GetArg("-m", "-map") ?? GameScene,
					AutoRun = AutoRunServer
			};

			var autoRun = GetArg("-autorun");
			if (string.IsNullOrEmpty(autoRun) == false)
			{
				try
				{
					config.AutoRun = Convert.ToBoolean(autoRun);
				}
				catch (InvalidCastException)
				{ }
			}

			return string.IsNullOrEmpty(config.Map) == false && BoltScenes.AllScenes.Contains(config.Map);
		}

		private bool IsHeadlessMode()
		{
			return Environment.CommandLine.Contains("-batchmode") && Environment.CommandLine.Contains("-nographics");
		}

		public IEnumerator ShutdownRoutine()
		{
			Debug.LogError("Application is about to quit");
			yield return new WaitForSeconds(5f);
			Application.Quit();
		}

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
		private void PlayFabStart() {}
		private void OnServerActive() {}
		#endif
	}
}