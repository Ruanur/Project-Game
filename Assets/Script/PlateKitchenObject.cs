using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    // 재료가 추가될 때 발생하는 이벤트 정의
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

    // 재료 추가 이벤트 인수 클래스 정의
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    // 유효한 재료 목록을 저장하는 리스트
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    // 현재 접시에 추가된 재료들을 저장하는 리스트
    private List<KitchenObjectSO> kitchenObjectSOList;

    // Awake 메서드: 초기 설정 수행
    protected override void Awake()
    {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    // 재료를 추가하려 시도하는 메서드
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        // 유효한 재료인지 확인
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // 유효하지 않은 재료
            return false;
        }

        // 이미 추가된 재료인지 확인
        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // 이미 추가된 재료
            return false;
        }
        else
        {
            // 재료를 서버에 추가
            AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
            return true;
        }
    }

    // 서버 측에서 재료를 추가하는 메서드 (서버 RPC)
    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        // 클라이언트 측에서 재료 추가 메서드 호출
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    // 클라이언트 측에서 재료를 추가하는 메서드 (클라이언트 RPC)
    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        // 인덱스를 통해 재료 정보 가져오기
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        // 재료를 리스트에 추가
        kitchenObjectSOList.Add(kitchenObjectSO);

        // 재료가 추가되었음을 알리는 이벤트 발생
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }

    // 현재 접시에 추가된 모든 재료의 목록을 반환하는 메서드
    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
}
