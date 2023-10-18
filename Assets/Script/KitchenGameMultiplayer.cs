using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class KitchenGameMultiplayer : NetworkBehaviour 
{
    public static KitchenGameMultiplayer Instance {get; private set;}

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    private void Awake()
    {
        Instance = this; //싱글톤 패턴으로 현재 인스턴스 설정.
    }
    //싱글톤

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRPC(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    } 
    
    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRPC(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        //주방 오브젝트 생성
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        //NetworkObject를 얻어와 스폰(생성)
        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        //KitchenObject 컴포넌트를 가져와 부모 설정
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }
    //서버간 오브젝트 생성 동기화

    private int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        //KitchenObjectSO를 목록에서 찾아 인덱스 반환
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    private KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
    {
        //주어진 인덱스를 사용해 KitcheneObjectSO를 반환
        return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
    }


    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);

    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);
        
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        kitchenObject.ClearKitchenObjectOnParent();
    }
}
