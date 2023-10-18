using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
    //요리 진행 상황 이벤트를 처리하는 이벤트 핸들러 
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    //재료(패티) 상태 변경 이벤트를 처리하는 이벤트 핸들러 
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    //오브젝트(패티)의 상태를 열거
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray; //FryingRecipeSO 배열
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray; //BurningRecipeSO 배열

    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private FryingRecipeSO fryingRecipeSO;
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private BurningRecipeSO burningRecipeSO;



    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f; //replace
        //요리 진행 상황 이벤트 호출
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer.Value / fryingTimerMax
        });

    }

    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f; //replace
        //요리 진행 상황 이벤트 호출
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value / burningTimerMax
        });

    }

    private void State_OnValueChanged(State previousState, State newState)
    {
        //상태 변경 이벤트 호출
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value
        });

        if (state.Value == State.Burned || state.Value == State.Idle)
        {
            //진행 상황 초기화
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
        }
    }

    private void Update()
    {
        if(!IsServer)
        {
            return;
        }
        if (HasKitchenObject())
        {
            switch (state.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;

                    if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax)
                    {
                        //Fried
                        //요리가 완료되면 요리 결과 생성
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        //상태를 Fried로 변경하고, Burning 레시피 설정
                        state.Value = State.Fried;
                        burningTimer.Value = 0f;
                        SetBurningRecipeSOClientRpc(
                            KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO())
                        );
                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    }
                    break;

                case State.Fried:
                    burningTimer.Value += Time.deltaTime;


                    if (burningTimer.Value> burningRecipeSO.burningTimerMax)
                    {
                        //Fried
                        //요리가 완료되면 요리 결과 생성 
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        //상태를 Burned로 변경
                        state.Value = State.Burned;
                    }
                    break;

                case State.Burned:
                    break; 
            }
        }
    }

    //플레이어와 상호 작용 처리 
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //요리대에 아무것도 없고
            if (player.HasKitchenObject())
            {
                //플레이어가 요리 재료를 가지고 있으면
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //플레이어가 가지고 있는 것을 요리함
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc(
                        KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO())
                    );
                }
            }
            else
            {
                //플레이어가 아무것도 가지고 있지 않은 경우 아무런 수행도 하지 않는다.
            }

        }
        else
        {
            //요리대에 요리 재료가 있으며,
            if (player.HasKitchenObject())
            {
                //플레이어가 요리 재료를 가지고 있고, 
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //플레이어가 접시를 가지고 있으면
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        //요리가 완료되었을 때 요리 결과를 생성하고 요리대에서 제거
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        SetStateIdleServerRpc();
                    }
                }
            }
            else
            {
                //플레이어가 아무것도 가지고 있지 않은 경우
                GetKitchenObject().SetKitchenObjectParent(player);

                SetStateIdleServerRpc();
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }


    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        fryingTimer.Value = 0f;
        state.Value = State.Frying;


        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        //Frying 레시피 설정 및 상태를 Frying로 변경 
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        //Frying 레시피 설정 및 상태를 Frying로 변경 
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
    }

    //입력에 해당하는 레시피가 있는지 확인
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingrecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingrecipeSO != null;
    }

    //입력에 해당하는 출력을 반환
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    //입력에 해당하는 Frying 레시피를 찾아서 반환
    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    //입력에 해당하는 Burning 레시피를 찾아서 반환
    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }

    //요리가 Fried 상태인지 여부 확인 
    public bool IsFried()
    {
        return state.Value == State.Fried;
    }
}
