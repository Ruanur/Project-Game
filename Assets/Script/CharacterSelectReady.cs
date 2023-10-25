using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance {get; private set;}


    public event EventHandler OnReadyChanged;

    //각 플레이어의 준비 상태를 저장하는 딕셔너리
    //Dictionary = 키-값 쌍의 컬렉션을 저장하고 관리하는데 사용. ulong = 클라이언트 id, bool = 준비 상태
    private Dictionary<ulong, bool> playerReadyDictionary;


    private void Awake()
    {
        Instance = this;

        //플레이어의 준비 상태를 저장할 딕셔너리 초기화
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }


    //클라이언트에서 사용하는 메서드: 플레이어를 준비 상태로 변경 요청
    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }
    

    //서버에서 실행되는 RPC 메소드 : 플레이어의 준비 상태 변경 요청 처리
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //클라이언트의 준비 상태 변경 처리
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        //플레이어의 준비 상태를 딕셔너리에 저장
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        //모든 클라이언트가 준비되었는지 확인 
        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                //단 한명의 플레이어라도 준비되어 있지 않으면, 모든 클라이언트가 준비되지 않은 상태로 간주
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            //모든 클라이언트가 준비되었을 떄 게임 시작
            KitchenGameLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    //클라이언트에서 실행되는 RPC 메소드 : 클라이언트에세 플레이어의 준비 상태 변경 통지
    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        //플레이어의 준비 상태를 업데이트하고 이벤트 호출
        playerReadyDictionary[clientId] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    } 

    //특정 클라이언트의 준비 상태 확인
    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}   