using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerCounter : BaseCounter
{
    //플레이어가 물건을 집었을 때 발생하는 이벤트
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    //플레이어와 상호작용하는 메소드 재정의
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //플레이어가 오브젝트를 가지고 있지 않을 때,
            //오브젝트를 생성하고 플레이어에게 할당한다.
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            //서버에서 InteractLogicServerRpc 메소드 호출
            InteractLogicServerRpc();
        }
    }
    
    //서버에서 호출되는 RPC 메소드, 클라이언트에게 InteractLogicClientRpc 메소드를 호출하도록 요청
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    //클라이언트에서 호출되는 RPC 메소드, OnPlayerGrabbedObject 이벤트 호출
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
    
}
