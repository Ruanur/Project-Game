using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.Netcode;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobby_OnJoinFailed;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobby_OnQuickJoinFailed;

        Hide();
    }

    private void KitchenGameLobby_OnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("로비를 찾지 못했습니다!");
    }

    private void KitchenGameLobby_OnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("로비 참여에 실패했습니다!");
    }

    private void KitchenGameLobby_OnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("로비 참여 중...");
    }

    private void KitchenGameLobby_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("로비 생성에 실패했습니다!");
    }

    private void KitchenGameLobby_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("로비 생성 중...");
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        if(NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("연결에 실패했습니다.");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
        Show();

        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if(messageText.text == "")
        {
            messageText.text = "연결에 실패했습니다.";
        }
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy() 
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnJoinFailed -= KitchenGameLobby_OnJoinFailed;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenGameLobby_OnQuickJoinFailed;
    }
}
