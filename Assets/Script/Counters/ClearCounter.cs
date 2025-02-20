using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    //Player와 상호작용하는 메소드 재정의
    public override void Interact(Player player)
    {
        //카운터에 KitchenObject가 없을 때
        if (!HasKitchenObject())
        { 
            if (player.HasKitchenObject())
            {
                //플레이어가 오브젝트를 지니고 있을 때, 현재 오브젝트를 이 위치에 설정
                player.GetKitchenObject().SetKitchenObjectParent(this); 
            }
            else
            {
                //플레이어가 오브젝트를 지니고 있지 않을 때, 아무 작업도 하지 않는다.
            }

        }
        else
        {
           //오브젝트가 있을 때
           if(player.HasKitchenObject())
            {
                //플레이어가 오브젝트를 옮길 때
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //플레이어가 접시를 가지고 있을 때
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        //플레이어의 접시에 현재 카운터에 있는 오브젝트(재료)를 접시에 추가하고, 놓여있던 오브젝트 파괴(삭제)
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
                else
                {
                    //플레이어가 접시를 가지고 있지 않을 때
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        //카운터가 접시를 가지고 있을 때(카운터에 접시가 놓여있을 때)
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            //카운터의 접시에 플레이어의 오브젝트(재료)를 추가하고, 플레이어의 오브젝트를 파괴(삭제)
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    }
                }
            } 
            else
            {
                //플레이어가 오브젝트를 가지고 있지 않을 때, 현재 오브젝트를 플레이어의 오브젝트로 설정.
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}