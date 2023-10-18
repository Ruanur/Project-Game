using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    //DeliveryCounter의 단일 인스턴스를 가져오는 정적 속성 
    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        //클래스의 인스턴스를 설정 
        Instance = this;
    }

    //플레이어와 상호작용 하는 메소드 재정의
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            //플레이어가 오브젝트를 가지고 있을 때
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                //접시만 허용
                //DeliveryManager를 이용해 레시피를 전달하고 접시를 파괴한다. 
            {
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }
}
