using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //Player is not Carrying anything
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            InteractLogicServerRpc();
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
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
    //컨테이너 카운터 동기화, 애니메이션
}
