using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{

    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton; //강퇴 버튼
    [SerializeField] private TextMeshPro playerNameText; //플레이어 이름 표시 텍스트

    private void Awake()
    {
        //추방 버튼 클릭시 실행되는 리스너 
        //리스너 -> 특정 이벤트의 발생을 감지하고 그에 따른 동작을 처리하는 역할을 한다.
        kickButton.onClick.AddListener(() => {
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            KitchenGameLobby.Instance.KickPlayer(playerData.playerId.ToString());
            KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        //플레이어 데이터 변경 및 준비 상태 변경 이벤트에 대한 핸들러 등록
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChaged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        //서버 여부에 따라 플레이어 퇴장 버튼 활성화
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        //플레이어 정보 업데이트
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
    {
        //준비 상태 변경 이벤트 
        UpdatePlayer();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChaged(object sender, EventArgs e)
    {
        //플레이어 데이터 네트워크 리스트 변경 이벤트
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        //플레이어 정보 이벤트 
        if(KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            //연결된 플레이어 정보 표시
            Show();

            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);

            //플레이어의 준비 상태에 따라 오브젝트 표시 여부 결정
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

            playerNameText.text = playerData.playerName.ToString(); //플레이어 이름 표시

            playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId)); //플레이어 비주얼 색상 변경
        }
        else
        {
            Hide();
        }
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
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChaged;
    }
}
