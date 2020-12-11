using System;
using Photon.Bolt;
using Photon.Bolt.Utils;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Bolt.Samples.Photon.Lobby
{
	public class LobbyPlayer : EntityEventListener<ILobbyPlayerInfoState>
	{
		// Bolt
		public BoltConnection connection;

		public bool IsReady
		{
			get { return state.Ready; }
		}

		// Lobby
		public string playerName
		{
			get { return nameInput.text; }
		}

		public Color playerColor
		{
			get { return colorButton.GetComponent<Image>().color; }
			set { colorButton.GetComponent<Image>().color = value; }
		}

		public bool ready = false;

		public Button colorButton;
		public InputField nameInput;
		public Button readyButton;
		public Button waitingPlayerButton;
		public Button removePlayerButton;

		public GameObject localIcone;
		public GameObject remoteIcone;

		public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
		public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

		static Color JoinColor = new Color(255.0f / 255.0f, 0.0f, 101.0f / 255.0f, 1.0f);
		static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
		static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
		static Color TransparentColor = new Color(0, 0, 0, 0);

		public static LobbyPlayer localPlayer;

		public override void Attached()
		{
			state.AddCallback("Name", () => nameInput.text = state.Name);
			state.AddCallback("Color", () => playerColor = state.Color);
			state.AddCallback("Ready", callback: () => OnClientReady(state.Ready));

			if (entity.IsOwner)
			{
				state.Color = Random.ColorHSV();
				state.Name = string.Format("{0} #{1}", GenerateFullName(), Random.Range(1, 100));
				state.Ready = ready = false;
			}
		}

		public override void ControlGained()
		{
			BoltLog.Info("ControlGained");

			readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
			SetupPlayer();
		}

		public void OnPlayerListChanged(int idx)
		{
			GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
		}

		public void OnRemovePlayerClick()
		{
			if (BoltNetwork.IsServer)
			{
				LobbyPlayerKick.Create(entity, EntityTargets.OnlyController).Send();
			}
		}

		public override void OnEvent(LobbyPlayerKick evnt)
		{
			BoltNetwork.Shutdown();
		}

		public override void SimulateController()
		{
			// Update every 5 frames
			if (BoltNetwork.Frame % 5 != 0) return;

			var input = LobbyCommand.Create();

			input.Name = playerName;
			input.Color = playerColor;
			input.Ready = ready;

			entity.QueueInput(input);
		}

		public override void ExecuteCommand(Command command, bool resetState)
		{
			if (!entity.IsOwner) { return; }

			if (!resetState && command.IsFirstExecution)
			{
				LobbyCommand lobbyCommand = command as LobbyCommand;

				state.Name = lobbyCommand.Input.Name;
				state.Color = lobbyCommand.Input.Color;
				state.Ready = lobbyCommand.Input.Ready;
			}
		}

		// Commands

		public void SetupOtherPlayer()
		{
			BoltLog.Info("SetupOtherPlayer");

			nameInput.interactable = false;

			removePlayerButton.gameObject.SetActive(BoltNetwork.IsServer);
			removePlayerButton.interactable = BoltNetwork.IsServer;

			ChangeReadyButtonColor(NotReadyColor);

			readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
			readyButton.interactable = false;

			OnClientReady(state.Ready);
		}

		public void SetupPlayer()
		{
			BoltLog.Info("SetupPlayer");

			localPlayer = this;

			nameInput.interactable = true;
			remoteIcone.gameObject.SetActive(false);
			localIcone.gameObject.SetActive(true);

			removePlayerButton.gameObject.SetActive(false);
			removePlayerButton.interactable = false;

			ChangeReadyButtonColor(JoinColor);

			readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
			readyButton.interactable = true;

			//we switch from simple name display to name input
			colorButton.interactable = true;
			nameInput.interactable = true;

			nameInput.onEndEdit.RemoveAllListeners();
			//            nameInput.onEndEdit.AddListener((text => { playerName = text; }));

			colorButton.onClick.RemoveAllListeners();
			colorButton.onClick.AddListener(() => { playerColor = Random.ColorHSV(); });

			readyButton.onClick.RemoveAllListeners();
			readyButton.onClick.AddListener(OnReadyClicked);

			OnClientReady(state.Ready);
		}

		public void RemovePlayer()
		{
			if (entity && entity.IsAttached)
			{
				BoltNetwork.Destroy(gameObject);
			}
		}

		public override void Detached()
		{
			//            if (OnDetach != null) OnDetach.Invoke(this);
		}
		// Utils

		private string GenerateFullName()
		{
			return string.Format("{0} {1}",
					GenerateName(new System.Random(DateTime.Now.Second - 1000).Next(4, 10)),
					GenerateName(new System.Random(DateTime.Now.Second + 1000).Next(4, 10))
			);
		}

		private string GenerateName(int len)
		{
			var rand = new System.Random(DateTime.Now.Second);

			string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
			string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
			string name = "";

			name += consonants[rand.Next(consonants.Length)].ToUpper();
			name += vowels[rand.Next(vowels.Length)];

			var b = 2;
			while (b < len)
			{
				name += consonants[rand.Next(consonants.Length)];
				b++;
				name += vowels[rand.Next(vowels.Length)];
				b++;
			}

			return name;
		}

		// UI

		void ChangeReadyButtonColor(Color c)
		{
			ColorBlock b = readyButton.colors;
			b.normalColor = c;
			b.pressedColor = c;
			b.highlightedColor = c;
			b.disabledColor = c;
			readyButton.colors = b;
		}

		public void OnReadyClicked()
		{
			ready = !ready;
		}

		public void OnClientReady(bool readyState)
		{
			if (readyState)
			{
				ChangeReadyButtonColor(TransparentColor);

				Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
				textComponent.text = "READY";
				textComponent.color = ReadyColor;
				readyButton.interactable = false;
				colorButton.interactable = false;
				nameInput.interactable = false;
			}
			else
			{
				ChangeReadyButtonColor(entity.IsControlled ? JoinColor : NotReadyColor);

				Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
				textComponent.text = entity.IsControlled ? "JOIN" : "...";
				textComponent.color = Color.white;
				readyButton.interactable = entity.IsControlled;
				colorButton.interactable = entity.IsControlled;
				nameInput.interactable = entity.IsControlled;
			}
		}
	}

}
