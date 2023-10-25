using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Unity.Netcode;

//레시피 관리 및 서빙을 처리하는 데 사용.
public class DeliveryManager : NetworkBehaviour 
{
    //레시피 생성, 성공, 실패 이벤트
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;


    public static DeliveryManager Instance { get; private set; }


    //레시피 목록 스크립트 객체 
    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList; //대기 중인 레시피 목록
    private float spawnRecipeTimer = 4f; //레시피 생성 주기 타이머
    private float spawnRecipeTimerMax = 4f; //레시피 생성 주기 최대값
    private int waitingRecipesMax = 4; //최대 대기 레시피 수
    private int successfulRecipesAmount; //성공한 레시피 수

    private void Awake()
    {
        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>(); //대기중인 레시피 목록 초기화

        
    }
    private void Update()
    {
        if(!IsServer)
        {
            return; //서버가 아닌 클라이언트에서는 업데이트 하지 않음, 호스트에서만 레시피를 생성, 클라이언트에서는 호스트의 레시피를 따름
        }

        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax)
            {
                //RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)]; //랜덤으로 레시피 제시
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
        }
    }

    //클라이언트에 새로운 대기 중인 레시피 생성 
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];

        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    } //클라이언트 레시피 동기화


    //플레이어가 레시피를 배달할 떄 호출되는 함수
    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {   
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                //동일한 수의 재료를 가지고 있는 경우
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    //레시피의 모든 재료를 순환하면서 확인
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        //판에 있는 모든 재료를 순환하면서 확인
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            //재료가 일치하는 경우
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        //이 레시피 재료가 접시에 없는 경우
                        plateContentsMatchesRecipe = false;
                    }
                }
                if (plateContentsMatchesRecipe)
                {
                    //플레이어가 올바른 레시피를 제공할 경우
                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }
        //일치하는 레시피를 찾을 수 없을 때. 
        //플레이어가 올바른 레시피를 제공하지 않았을 때.
        DeliverInCorrectRecipeServerRpc();
    }
    
    //서버에서 호출되는 레시피 배달 실패 함수
    [ServerRpc(RequireOwnership = false)] //소유권 해제, 클라이언트 서버에서 배달 성공했을 때 알림이 뜨게 하기 위함
    private void DeliverInCorrectRecipeServerRpc()
    {
        DeliverInCorrectRecipeClientRpc();
    }


    //클라이언트에서 호출되는 레시피 배달 실패 함수
    [ClientRpc]
    private void DeliverInCorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }


    //서버에서 호출되는 올바른 레시피를 배달한 경우 
    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    //클라이언트에서 호출되는 올바른 레시피를 배달한 경우
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        successfulRecipesAmount++;

        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }//성공 레시피 동기화

    //대기 중인 레시피 목록 리턴
    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    //성공한 레시피 수 리턴
    public int GetSuccessfulRecipesAmount()
    {
        return successfulRecipesAmount;
    }

}
