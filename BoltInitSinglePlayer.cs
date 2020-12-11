using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bolt.Samples
{
	public class BoltInitSinglePlayer : GlobalEventListener
	{
		enum State
		{
			SelectMode,
			SelectMap,
			StartServer,
			Started,
		}

		State state;
		string map;

		void OnGUI()
		{
			Rect tex = new Rect(10, 10, 140, 75);
			Rect area = new Rect(10, 90, Screen.width - 20, Screen.height - 100);

			GUI.Box(tex, Resources.Load("BoltLogo") as Texture2D);
			GUILayout.BeginArea(area);

			switch (state)
			{
				case State.SelectMode: State_SelectMode(); break;
				case State.SelectMap: State_SelectMap(); break;
				case State.StartServer: State_StartSinglePlayer(); break;
			}

			GUILayout.EndArea();
		}

		void State_SelectMode()
		{
			if (ExpandButton("Start Single Player"))
			{
				state = State.SelectMap;
			}
		}

		void State_SelectMap()
		{
			GUILayout.BeginVertical();

			foreach (string value in BoltScenes.AllScenes)
			{
				if (SceneManager.GetActiveScene().name != value)
				{
					if (ExpandButton(value))
					{
						map = value;
						state = State.StartServer;
					}
				}
			}

			GUILayout.EndVertical();
		}

		void State_StartSinglePlayer()
		{
			BoltLauncher.StartSinglePlayer();
			state = State.Started;
		}

		public override void BoltStartDone()
		{
			if (BoltNetwork.IsServer)
			{
				var id = Guid.NewGuid().ToString().Split('-')[0];
				var matchName = string.Format("{0} - {1}", id, map);

				BoltMatchmaking.CreateSession(
					sessionID: matchName, 
					sceneToLoad: map
				);
			}
		}

		bool ExpandButton(string text)
		{
			return GUILayout.Button(text, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		}
	}
}
