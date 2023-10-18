using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    //휴지통에서 물체가 폐기될 때 발생하는 이벤트 
    public static event EventHandler OnAnyObjectTrashed;

    //정적 데이터를 초기화하는 메소드 
    new public static void ResetStaticData()
    {
        OnAnyObjectTrashed = null;
    }

    //플레이어와 상호작용하는 메소드 
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            //플레이어가 버리는 물체를 파괴

            InteractLogicServerRpc();
            //물체가 폐기될 때 이벤트를 발생시킴 
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }
}
