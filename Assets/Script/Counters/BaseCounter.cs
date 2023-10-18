using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{

    public static event EventHandler OnAnyObjectPlacedHere;


    //정적 데이터 초기화
    public static void ResetStaticData()
    {
        OnAnyObjectPlacedHere = null;
    }



    [SerializeField] private Transform counterTopPoint;


    private KitchenObject kitchenObject;

    //Player와 상호작용 
    public virtual void Interact(Player player)
    {
        Debug.LogError("BaseCounter.Interact();");
    }

    public virtual void InteractAlternate(Player player)
    {
        //Debug.LogError("BaseCounter.InteractAlternate();");
    }

    //오브젝트가 따라가는 위치 반환 
    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    //오브젝트 설정 
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null ) 
        {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }

    //오브젝트 리턴
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    //오브젝트 클리어
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    //오브젝트의 유무 반환
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    //네트워크 오브젝트 리턴
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

}