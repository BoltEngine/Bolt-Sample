using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Steamworks;
using UdpKit;
using Bolt;
using System.Text;

public class SteamToken : Bolt.IProtocolToken
{
  public ulong SteamID;

  public void Write(UdpKit.UdpPacket packet)
  {
    packet.WriteULong(SteamID);
  }

  public void Read(UdpKit.UdpPacket packet)
  {
    SteamID = packet.ReadULong();
  }
}

public class SteamLobby : MonoBehaviour
{
  public GameObject createLobbyButton;
  public GameObject lobbyCanvas;
  public GameObject noActiveLobbyPanel;
  public GameObject ActiveLobbyPanel;
  public GameObject createLobbyText;
  public GameObject lobbyName;
  public GameObject lobbySlots;
  public GameObject chatInputField;
  public GameObject chatBox;
  public GameObject LobbyDetailsPanel;
  public GameObject chatMessagePrefab;
  public GameObject LobbyDetailsRowPanelPrefab;
  public GameObject LobbyMembersListPanel;
  public GameObject LobbyMemberDetailsRowPanelPrefab;
  public GameObject steamNotification;
  public GameObject currentEloText;
  public GameObject eloField;


  static string KICK_LOBBY_MESSAGE_COMMAND = "/kick ";   // INCLUDES trailing/delimiting space if it takes params

  public CSteamID lobbyGameServerSteamId;                // Set in steamPeer_Host_Connected Callback
  ushort serverPort = 25000;

  private static int lobbyEnteredCounter = 0;            // Just some dummy code to see if callbacks fire

  public Image profilePic;
  public Text profileName;
  private Texture2D m_LargeAvatar;

  [SerializeField]
  private GameObject notificationPanel;

  [SerializeField]
  private GameObject eloPanel;

  [SerializeField]
  private bool debugMode;

  [SerializeField]
  private Image currentEloVisualFeedback;

  [SerializeField]
  private GameObject noMessagesHint;

  void Awake()
  {
    // Activating or deactivating childs in the notification panel
    foreach (Transform t in notificationPanel.GetComponentsInChildren<Transform>())
    {
      t.gameObject.SetActive(debugMode);
    }

    // Activating or deactivating childs in the ELO panel
    foreach (Transform t in eloPanel.GetComponentsInChildren<Transform>())
    {
      t.gameObject.SetActive(debugMode);
    }

    notificationPanel.SetActive(true);
    eloPanel.SetActive(true);
  }

  void Update()
  {

    SteamAPI.RunCallbacks();

    if (lobbyName.GetComponent<Text>().text != "" && lobbySlots.GetComponent<Text>().text != "")
    {
      createLobbyButton.GetComponent<Button>().interactable = true;
    }
    else
    {
      createLobbyButton.GetComponent<Button>().interactable = false;
    }

    var activeLobby = SteamHub.LobbyActive;

    if (BoltNetwork.IsRunning != true)
    {
      if (activeLobby == null)
      {
        noActiveLobbyPanel.SetActive(true);
        ActiveLobbyPanel.SetActive(false);
      }
      else
      {
        noActiveLobbyPanel.SetActive(false);
        ActiveLobbyPanel.SetActive(true);
      }
    }
    else
    {
      noActiveLobbyPanel.SetActive(false);
      ActiveLobbyPanel.SetActive(false);
    }
  }


  // Use this for initialization
  void Start()
  {
    DontDestroyOnLoad(lobbyCanvas);

    // fill out all callbacks on SteamHub.Lobby_Callbacks
    var callbacks = new SteamHub.Lobby_Callbacks
    {
      LobbyCreated = MyCallbacks_LobbyCreated,
      LobbyEntered = MyCallbacks_LobbyEntered,
      LobbyListUpdated = MyCallbacks_LobbyListUpdated,
      LobbyDataUpdated = MyCallbacks_LobbyDataUpdated,
      LobbyChatUpdate = MyCallbacks_LobbyChatUpdate,
      LobbyChatMessage = MyCallbacks_LobbyChatMessage
    };

    // call LobbyManager_Start
    SteamHub.LobbyManager_Start(callbacks);

    Text t = createLobbyText.GetComponent<Text>();
    t.text = "Create or join a lobby please";

    SteamHub.Lobby_RefreshList();

    if (SteamManager.Initialized && profilePic != null)
    {
      profileName.text = SteamFriends.GetPersonaName();
      SteamFriends.SetRichPresence("status", "Main Menu");

      //display profile pic in top right
      CSteamID m_Friend = SteamUser.GetSteamID();

      int FriendAvatar = SteamFriends.GetLargeFriendAvatar(m_Friend);
      print("SteamFriends.GetLargeFriendAvatar(" + m_Friend + ") - " + FriendAvatar);

      uint ImageWidth;
      uint ImageHeight;
      bool ret = Steamworks.SteamUtils.GetImageSize(FriendAvatar, out ImageWidth, out ImageHeight);

      if (ret && ImageWidth > 0 && ImageHeight > 0)
      {
        byte[] Image = new byte[ImageWidth * ImageHeight * 4];

        ret = Steamworks.SteamUtils.GetImageRGBA(FriendAvatar, Image, (int)(ImageWidth * ImageHeight * 4));
        if (ret)
        {
          m_LargeAvatar = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
          m_LargeAvatar.LoadRawTextureData(Image);
          m_LargeAvatar.Apply();

          profilePic.GetComponent<Image>().sprite = Sprite.Create(m_LargeAvatar, new Rect(0, 0, 184, 184), new Vector2(184, 184));
        }
      }
    }

    if (SteamManager.Initialized)
    {
      Debug.Log("Steam Online");

      steamNotification.GetComponent<Text>().color = Color.green;
      steamNotification.GetComponent<Text>().text = "STEAM ONLINE";
      // steamNotification.SetActive(false);

      //bool ret0 = 
      SteamUserStats.RequestCurrentStats();

      //we're going to use Spacewar's "NumWins" to track kills and "AverageSpeed" as ELO for matchmaking
      //NumLosses as deaths?
      //float myELO;
      //SteamUserStats.GetStat("AverageSpeed", out myELO);
      //bool ret4 = SteamUserStats.ResetAllStats(true);
      int Data;
      //bool ret = 
      SteamUserStats.GetStat("NumGames", out Data);
      // Debug.Log(Data + " " + ret);

      currentEloText.GetComponent<Text>().text = "CURRENT ELO: " + Data;

      currentEloVisualFeedback.fillAmount = Data / 100.0f;

      // int m_nTotalNumWins;
      //SteamUserStats.GetStat("NumWins", out m_nTotalNumWins);
      //Debug.Log(m_nTotalNumWins);

      //SteamUserStats.SetStat("NumWins", 10);

      //bool bSuccess = SteamUserStats.StoreStats();
      //Debug.Log(bSuccess);
    }
    else
    {
      Debug.Log("Steam Offline");

      steamNotification.GetComponent<Text>().color = Color.red;
      steamNotification.GetComponent<Text>().text = "STEAM OFFLINE";
    }
  }

  private void MyCallbacks_LobbyChatUpdate(LobbyChatUpdate_t chatUpdate)
  {
    // Debug.Log("MyCallbacks_LobbyChatUpdate");
  }

  private void MyCallbacks_LobbyChatMessage(LobbyChatMsg_t chatMsg)
  {
    // Debug.Log("MyCallbacks_LobbyChatMessage");
    processLobbyMessage(chatMsg);
  }

  private void MyCallbacks_LobbyDataUpdated(SteamHubLobby lobby)
  {
    //don't refresh list here, can cause infinite loop
  }

  public CSteamID getGameServerID()
  {
    return SteamHub.LobbyActive.ServerId;
  }

  public bool isOwner()
  {
    return SteamHub.LobbyActive.IsOwner;
  }

  /// UI Methods

  public void ui_reset_stats()
  {
    SteamUserStats.ResetAllStats(true);

    int Data;
    bool ret = SteamUserStats.GetStat("NumGames", out Data);
    Debug.Log(Data + " " + ret);

    currentEloText.GetComponent<Text>().text = "CURRENT ELO: " + Data;
  }

  public void ui_set_ELO()
  {
    string stringELO = eloField.GetComponent<Text>().text;
    if (stringELO != "")
    {
      int newELO = Convert.ToInt32(stringELO);
      //currentEloText.GetComponent<Text>().text = "CURRENT ELO: " + newELO.ToString();

      SteamUserStats.RequestCurrentStats();

      int Data;
      bool ret = SteamUserStats.GetStat("NumGames", out Data);

      if (newELO < Data)
      {
        SteamUserStats.ResetAllStats(true);
      }

      if (ret)
      {
        SteamUserStats.SetStat("NumGames", newELO);
        currentEloText.GetComponent<Text>().text = "CURRENT ELO: " + newELO;

        currentEloVisualFeedback.fillAmount = newELO / 100.0f;
      }
    }
  }

  public void ui_find_match()
  {
    //print("SetStat(\"NumGames\", " + (4) + ") - " + ret);

    //SteamUserStats.GetStat("NumGames", out m_NumGamesStat);
    //Debug.Log(ret + " " + m_NumGamesStat);

    //bool ret = SteamUserStats.SetStat("NumGames", m_NumGamesStat + 1);
    //print("SetStat(\"NumGames\", " + (m_NumGamesStat + 1) + ") - " + ret);

    //Debug.Log(m_NumGamesStat);
    //float myELO;
    //SteamUserStats.GetStat("AverageSpeed", out myELO);

    //Debug.Log(myELO);
    int Data;
    bool ret = SteamUserStats.GetStat("NumGames", out Data);
    if (ret)
    {
      //SteamMatchmaking.AddRequestLobbyListNearValueFilter("ELO", Data);
      SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
      SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterDefault);

      SteamMatchmaking.AddRequestLobbyListResultCountFilter(10);

      SteamHub.Lobby_RefreshList();
    }
  }

  /// <summary>
  /// Join match based on ELO, game mode, etc
  /// host new game if none found
  /// </summary>
  public void ui_join_match()
  {
    foreach (var lobby in SteamHub.LobbyList)
    {
      int lobbyMaxMembers = SteamMatchmaking.GetLobbyMemberLimit(lobby.LobbyId);

      if (lobby.MembersCount < lobbyMaxMembers)
      {
        SteamHub.Lobby_Join(lobby);
      }

      break;
    }
  }

  public void ui_do_createLobby()
  {
    if (lobbyName.GetComponent<Text>().text != "" && lobbySlots.GetComponent<Text>().text != "")
    {
      int desiredLobbySlots = int.Parse(lobbySlots.GetComponent<Text>().text);
      // Create a lobby
      SteamHub.Lobby_Create(false, desiredLobbySlots);
      Text t = createLobbyText.GetComponent<Text>();
      t.text = "Created Lobby with " + desiredLobbySlots.ToString() + " slots";
    }
    else
    {
      // TODO: Tell user to enter a name
    }
  }

  public void ui_do_sendLobbyMessage()
  {
    var activeLobby = SteamHub.LobbyActive;

    if (activeLobby != null)
    {
      if (chatInputField.GetComponent<Text>().text != "")
      {
        byte[] MsgAsBytes = Encoding.ASCII.GetBytes(SteamFriends.GetPersonaName() + ": " + chatInputField.GetComponent<Text>().text);
        SteamMatchmaking.SendLobbyChatMsg(SteamHub.LobbyActive.LobbyId, MsgAsBytes, MsgAsBytes.Length + 1);
        chatInputField.transform.parent.GetComponent<InputField>().text = "";
      }
      //Debug.Log(chatInputField.GetComponent<Text>().text);
    }
  }

  public void ui_do_getLobbies()
  {
    // Refresh the lobby list. 
    // This will cause the callbacks.LobbyListUpdated callback to fire when done
    SteamHub.Lobby_RefreshList();
    // Text t = LobbyListText.GetComponent<Text>();
    // t.text = "Refreshing Lobby List";
  }

  public void ui_do_leaveLobby()
  {
    SteamHub.Lobby_LeaveActive();
    SteamHub.Lobby_RefreshList();
  }

  public void ui_enterGame()
  {
    var steamPeerCallbacks = new SteamPeer.Callbacks();
    steamPeerCallbacks.Host_Connected = steamPeer_Host_Connected;
    steamPeerCallbacks.Host_Disconnected = steamPeer_Host_Disconnected;
    steamPeerCallbacks.Host_Failed = steamPeer_Host_Failed;

    var steamPeerSettings = new SteamPeer.Settings();
    steamPeerSettings.Host_Address = 0;

    var activeLobby = SteamHub.LobbyActive;

    // If no active lobby... return
    if (activeLobby == null)
    {
      Debug.Log("No active lobby to start game for");    // TODO we should say something to user
      return;
    }

    if (activeLobby.GetData<string>(activeLobby.OwnerId.ToString()) == "false" && !activeLobby.IsOwner)
    {
      Debug.Log("Host has not started game yet");
      return;
    }

    if (activeLobby.IsOwner)
    {
      // B.1: If you are the lobby creator/owner, you should call 
      // SteamPeer.StartHost(settings, callbacks),
      steamPeerSettings.Product_Description = "Steamworks Test";
      steamPeerSettings.Product_Name = "SpaceWar";
      steamPeerSettings.Product_Version = "0.00001";
      steamPeerSettings.Host_GamePort = serverPort;
      steamPeerSettings.Host_Address = 0;
      SteamPeer.StartHost(steamPeerSettings, steamPeerCallbacks);

      // We will receive the SteamID the clients should connect to in the Host_Connected callback.
      // Do nothing else yet.. this flow will continue in that callback
    }
    else
    {
      // B.2: If you are a member of a lobby and the creator / owner has started it, 
      // you can connect to the SteamID of the game server by first calling 
      // SteamPeer.StartClient(settings, callbacks);

      SteamPeer.StartClient(steamPeerSettings, steamPeerCallbacks);
    }

    // After you have called StartHost / StartClient, you should call 
    // BoltLauncher.SetUdpPlatform(new SteamPlatform());
    // depending on if you are host/ server, you do not need to specify any addresses / ports here.
    BoltLauncher.SetUdpPlatform(new SteamPlatform());

    // Now Start BOLT with BoltLauncher.StartServer(); or BoltLauncher.StartClient();
    if (activeLobby.IsOwner == true)
    {
      BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, (ushort)serverPort));
    }
    else
    {
      BoltLauncher.StartClient();
    }

    // If you are the server, everything is done now.

    // If you are the client you need to connect to the Host by calling
    // BoltNetwork.Connect(lobbyGameServerSteamId.ToEndPoint());
    //if (activeLobby.IsOwner == false)
    //{
    //    BoltNetwork.Connect(activeLobby.LobbyId.ToEndPoint());
    //}
  }

  void steamPeer_Host_Failed()
  {
    Debug.Log("HOST FAILED CALLBACK");
  }

  void steamPeer_Host_Disconnected()
  {
    Debug.Log("HOST DISCONNECTED CALLBACK");
  }

  void steamPeer_Host_Connected(CSteamID lobby)
  {
    //Debug.Log("HOST CONNECTED CALLBACK: ID=" + (lobby == null ? "<null>" : lobby.m_SteamID.ToString()));
    lobbyGameServerSteamId = lobby;
    SteamHub.LobbyActive.ServerId = lobby;        // TODO - decide if we can just use serverid instead
  }

  void MyCallbacks_LobbyEntered(SteamHubLobby lobby)
  {
    SteamHub.Lobby_RefreshList();
    SteamHub.LobbyActive.SetData<string>(SteamUser.GetSteamID().ToString(), "false");

    lobbyEnteredCounter++;

    Debug.Log("LobbyEntered called " + lobbyEnteredCounter.ToString() + " times. Last entered lobby # " + ((lobby == null) ? "NULL" : lobby.LobbyId.ToString()));

    foreach (Transform child in chatBox.transform)
    {
      Destroy(child.gameObject);
    }
  }

  void MyCallbacks_LobbyCreated(SteamHubLobby lobby)
  {
    int elo;
    //bool ret = 
    SteamUserStats.GetStat("NumGames", out elo);

    //if (ret)
    //    lobby.SetData<string>("ELO", ret);

    lobby.SetData<string>("name", lobbyName.GetComponent<Text>().text);
    Debug.Log("LOBBY CREATED CALLBACK: Lobby name\"" + lobby.GetData<string>("name") + "\"");
  }

  void MyCallbacks_LobbyListUpdated()
  {
    // Clear the Lobby List and Members List Panels
    foreach (Transform child in LobbyDetailsPanel.transform)
    {
      Destroy(child.gameObject);
    }

    foreach (Transform child in LobbyMembersListPanel.transform)
    {
      Destroy(child.gameObject);
    }

    // Instantiate the gameobject for the Each Lobby panel row
    foreach (var lobby in SteamHub.LobbyList)
    {
      string lobbyName = lobby.GetData<String>("name");
      if (lobbyName == null || lobbyName == "")
      {
        lobbyName = "<unnamed>";
      }

      GameObject lobbyRow = (GameObject)Instantiate(LobbyDetailsRowPanelPrefab, new Vector3(1, 1, 1), Quaternion.identity);
      lobbyRow.transform.SetParent(LobbyDetailsPanel.transform, false);
      lobbyRow.transform.localScale = new Vector3(1, 1, 1);
      lobbyRow.name = "Lobby details row for Lobby ID #" + lobby.LobbyId.ToString();

      Text t2 = lobbyRow.transform.Find("Lobby Details Text").GetComponent<Text>();

      Boolean isLobbyActive = (SteamHub.LobbyActive != null && lobby.LobbyId == SteamHub.LobbyActive.LobbyId);

      int lobbyMaxMembers = SteamMatchmaking.GetLobbyMemberLimit(lobby.LobbyId);

      String str = isLobbyActive ? "(A) " : "     ";
      str += "Lobby \"" + lobbyName + "\" #";
      str += lobby.LobbyId.ToString() + ": ";
      // str += "[" + lobby.MembersCount.ToString() + "/" + lobbyMaxMembers.ToString() + "] ";
      str += lobby.IsOwner ? " (OWNER)" : "";

      t2.text = str;

      Text t1 = lobbyRow.transform.Find("NumberOfPlayerText").GetComponent<Text>();
      t1.text = lobby.MembersCount.ToString() + "/" + lobbyMaxMembers.ToString();

      Image fill = lobbyRow.transform.Find("VisualFeedback").GetComponent<Image>();
      fill.fillAmount = (float)lobby.MembersCount / lobbyMaxMembers;

      Button joinButton = lobbyRow.transform.Find("Join Button").GetComponent<Button>();
      if (isLobbyActive)
      {
        // Provide a leave button
        Text joinButtonLabelText = (Text)joinButton.transform.GetChild(0).GetComponent<Text>();
        joinButtonLabelText.text = "LEAVE";
        var lob = lobby;    // Capture it for the lambda
        joinButton.onClick.AddListener(() => ui_click_leaveLobbyButtonPressed(joinButton, lob));

        // Instantiate the gameobject for the Each Lobby panel row
        foreach (var m in lobby.AllMembers)
        {
          GameObject memberRow = (GameObject)Instantiate(LobbyMemberDetailsRowPanelPrefab, new Vector3(1, 1, 1), Quaternion.identity);
          memberRow.transform.SetParent(LobbyMembersListPanel.transform, false);
          memberRow.transform.localScale = new Vector3(1, 1, 1);
          memberRow.name = "Member details row for Member ID #" + m.m_SteamID.ToString();
          Text t3 = memberRow.transform.Find("Member Details Text").GetComponent<Text>();

          Image LargeAvatar = memberRow.transform.Find("ProfileImage").GetComponent<Image>();

          int FriendAvatar = SteamFriends.GetSmallFriendAvatar(m);
          uint ImageWidth;
          uint ImageHeight;
          bool ret = Steamworks.SteamUtils.GetImageSize(FriendAvatar, out ImageWidth, out ImageHeight);

          if (ret && ImageWidth > 0 && ImageHeight > 0)
          {
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            ret = Steamworks.SteamUtils.GetImageRGBA(FriendAvatar, Image, (int)(ImageWidth * ImageHeight * 4));
            if (ret)
            {
              m_LargeAvatar = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
              m_LargeAvatar.LoadRawTextureData(Image);
              m_LargeAvatar.Apply();

              LargeAvatar.GetComponent<Image>().sprite = Sprite.Create(m_LargeAvatar, new Rect(0, 0, 32, 32), new Vector2(32, 32));
            }
          }


          string mName = SteamFriends.GetFriendPersonaName(m);
          string inGame = "";
          if (lobby.GetData<string>(m.ToString()) == "true")
            inGame = " (IN GAME) ";
          t3.text = inGame + "Member \"" + mName + "\" ID #" + m.m_SteamID.ToString();

          Button kickButton = memberRow.transform.Find("Kick Button").GetComponent<Button>();
          if (lobby.IsOwner)
          {
            var lobKick = lobby;    // Capture it for the lambda that follows
            var memberToKick = m;
            kickButton.onClick.AddListener(() => ui_click_kickFromLobbyButtonPressed(kickButton, lobKick, memberToKick));
          }
          else
          {
            kickButton.interactable = false;
          }

          if (lobby.OwnerId == (CSteamID)m.m_SteamID)
          {
            kickButton.interactable = false;
          }
        }
      }
      else
      {
        // Provide a join button
        var lobJoin = lobby;    // Capture it for the lambda that follows
        joinButton.onClick.AddListener(() => ui_click_joinLobbyButtonPressed(joinButton, lobJoin));
      }
    }

    // Yay all done
  }

  public void ui_CreateOverlay()
  {
    // NOTE THAT THIS DOESN'T WORK IN THE UNITY EDITOR. GOTTA BUILD+RUN
    var activeLobby = SteamHub.LobbyActive;

    if (activeLobby != null)
    {
      //SteamFriends.ActivateGameOverlayInviteDialog(activeLobby.LobbyId);
      SteamFriends.ActivateGameOverlay("LobbyInvite");
    }
  }

  void ui_click_joinLobbyButtonPressed(Button b, SteamHubLobby lobby)
  {
    noMessagesHint.SetActive(true);

    b.interactable = false;
    SteamHub.Lobby_Join(lobby);
  }

  void ui_click_kickFromLobbyButtonPressed(Button b, SteamHubLobby lobby, CSteamID memberToKick)
  {
    b.interactable = false;
    Debug.Log(" Sending KICK to " + memberToKick.m_SteamID.ToString());
    // KICK is just a magic lobby message
    string kickMessage = KICK_LOBBY_MESSAGE_COMMAND + memberToKick.m_SteamID.ToString();
    byte[] kickMsgAsBytes = Encoding.ASCII.GetBytes(kickMessage);
    SteamMatchmaking.SendLobbyChatMsg(lobby.LobbyId, kickMsgAsBytes, kickMsgAsBytes.Length + 1);
  }

  void ui_click_leaveLobbyButtonPressed(Button b, SteamHubLobby lobby)
  {
    b.interactable = false;
    SteamHub.Lobby_LeaveActive();
  }

  void processLobbyMessage(LobbyChatMsg_t msg)
  {
    Debug.Log("Retrieving message " + msg.m_iChatID.ToString() + " for Lobby#" + msg.m_ulSteamIDLobby);

    CSteamID steamLobbyMessageSenderId;             // Will be set by SteamMatchmaking.GetLobbyChatEntry()
    byte[] chatMessageAsBytes = new byte[256];      // Will be passed to SteamMatchmaking.GetLobbyChatEntry()
    EChatEntryType chatEntryType;                   // Will be set by SteamMatchmaking.GetLobbyChatEntry()

    // Check there is an active lobby
    var activeLobby = SteamHub.LobbyActive;
    if (activeLobby == null)
    {
      return;
    }

    // See if message is to our active lobby
    if ((CSteamID)msg.m_ulSteamIDLobby != activeLobby.LobbyId)
    {
      Debug.Log("Not our active lobby so ignoring");
      return;
    }

    int messageLengthInBytes = SteamMatchmaking.GetLobbyChatEntry(activeLobby.LobbyId,
                                                (int)msg.m_iChatID,        // TODO: CHECK FOR OVERFLOW
                                                out steamLobbyMessageSenderId,
                                                chatMessageAsBytes,
                                                chatMessageAsBytes.Length,
                                                out chatEntryType);

    string chatMessageAsString = Encoding.ASCII.GetString(chatMessageAsBytes, 0, messageLengthInBytes);

    Debug.Log("Chat message is " + chatMessageAsString);

    if (chatMessageAsString.StartsWith(KICK_LOBBY_MESSAGE_COMMAND))
    {
      ulong userIdToKick = ulong.Parse(chatMessageAsString.Substring(KICK_LOBBY_MESSAGE_COMMAND.Length));
      Debug.Log("KICK COMMAND: KICK USER#" + userIdToKick.ToString());

      // Check if KICK was sent by Lobby owner
      if (msg.m_ulSteamIDUser == activeLobby.OwnerId.m_SteamID)
      {
        Debug.Log("KICK COMMAND: SENT BY LOBBY OWNER OK #" + msg.m_ulSteamIDUser);
        if (userIdToKick == SteamUser.GetSteamID().m_SteamID)
        {
          Debug.Log("KICK COMMAND: WE HAVE BEEN KICKED BY LOBBY OWNER OK #" + msg.m_ulSteamIDUser);
          ui_do_leaveLobby();
        }
      }
      else
      {
        Debug.Log("KICK COMMAND IGNORED - WAS SENT BY NON-OWNER #" + msg.m_ulSteamIDUser);
      }
      return;
    }

    if (chatMessageAsString.StartsWith("enteredGame"))
    {
      ulong userIdEnteredGame = ulong.Parse(chatMessageAsString.Substring(11));
      Debug.Log("ENTERED GAME: " + userIdEnteredGame);
      SteamHub.LobbyActive.SetData<string>(userIdEnteredGame.ToString(), "true");
      return;
    }

    noMessagesHint.SetActive(false);

    GameObject chatMessage = (GameObject)Instantiate(chatMessagePrefab, new Vector3(1, 1, 1), Quaternion.identity);
    chatMessage.transform.SetParent(chatBox.transform, false);
    // lobbyRow.transform.localScale = new Vector3(1, 1, 1);
    chatMessage.GetComponent<Text>().text = chatMessageAsString;
    chatMessage.name = "Lobby message for Lobby ID #" + SteamHub.LobbyActive.LobbyId.ToString();
  }
}
