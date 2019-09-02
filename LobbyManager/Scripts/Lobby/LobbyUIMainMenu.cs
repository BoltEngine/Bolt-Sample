using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.Photon.Lobby
{
    public class LobbyUIMainMenu : MonoBehaviour, ILobbyUI
    {
        public event Action OnCreateButtonClick;
        public event Action OnBrowseServerClick;
        public event Action OnJoinRandomClick;

        public string MatchName
        {
            get { return matchNameInput.text; }
        }
        
        [Header("Server UI")]
        [SerializeField] private InputField matchNameInput;
        [SerializeField] private Button createRoomButton;

        [Header("Client UI")]
        [SerializeField] private Button browseServersButton;
        [SerializeField] private Button joinRandomButton;
        
        public void OnEnable()
        {
            createRoomButton.onClick.RemoveAllListeners();
            createRoomButton.onClick.AddListener(() =>
            {
                if (OnCreateButtonClick != null) OnCreateButtonClick();
            });
            
            browseServersButton.onClick.RemoveAllListeners();
            browseServersButton.onClick.AddListener(() =>
            {
                if (OnBrowseServerClick != null) OnBrowseServerClick();
            });
            
            joinRandomButton.onClick.RemoveAllListeners();
            joinRandomButton.onClick.AddListener(() =>
            {
                if (OnJoinRandomClick != null) OnJoinRandomClick();
            });

            matchNameInput.text = Guid.NewGuid().ToString();
        }

        public void ToggleVisibility(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
