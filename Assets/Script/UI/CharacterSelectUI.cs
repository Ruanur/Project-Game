using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() => 
        {
            KitchenGameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuscene);
        });

        readyButton.onClick.AddListener(() => {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }   

    private void Start()
    {
        Lobby lobby = KitchenGameLobby.Instance.GetLobby();

        lobbyNameText.text = "로비 이름: " + lobby.Name;
        lobbyCodeText.text = "로비 코드: " + lobby.LobbyCode;
    }

        

}
