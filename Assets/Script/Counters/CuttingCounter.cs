using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    //칼질(손질) 작업 중 어떤 작업이든 발생할 때 호출되는 이벤트 
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    private int cuttingProgress;

    //플레이어와 상호작용하는 메소드 재정의
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //오브젝트가 없고
            if (player.HasKitchenObject())
            {
                //플레이어가 오브젝트를 가지고 있으며,
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //플레이어가 가지고 있는 오브젝트가 자를 수 있는 경우
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { 
                    progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMAX});
                }
            }
            else
            {
                //플레이어가 아무것도 가지고 있지 않을 때는 아무런 수행을 하지 않는다.
            }

        }
        else
        {
            //주방 객체가 있고,
            if (player.HasKitchenObject())
            {
                //플레이어가 오브젝트를 가지고 있으며,
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //그 오브젝트가 접시일 때,
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        //플레이어의 접시에 현재 카운터의 오브젝트를 추가하고 카운터의 오브젝트를 파괴(삭제)한다.
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
            else
            {
                //플레이어가 아무것도 가지고 있지 않을 때, 현재 오브젝트를 플레이어의 오브젝트로 설정한다.
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    //대체 상호작용(재료 손질 - Key : F) 메소드 재정의
    public override void InteractAlternate(Player player)
    {
        if(HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //오브젝트가 있고 자를 수 있는 오브젝트 일 때
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMAX
            });

            if (cuttingProgress>= cuttingRecipeSO.cuttingProgressMAX)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                GetKitchenObject().DestroySelf();


                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
