using System;
using System.Collections;
using System.Collections.Generic;
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

    private State state;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;
    private float burningTimer;
    private BurningRecipeSO burningRecipeSO;


    private void Start()
    {
        state = State.Idle; //시작 시 상태를 Idle로 설정
    }
    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    //요리 진행 상황 이벤트 호출
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });

                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {
                        //Fried
                        //요리가 완료되면 요리 결과 생성
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        //상태를 Fried로 변경하고, Burning 레시피 설정
                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        //상태 변경 이벤트 호출
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    //요리 진행 상황 이벤트 호출
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                    });

                    if (burningTimer > burningRecipeSO.burningTimerMax)
                    {
                        //Fried
                        //요리가 완료되면 요리 결과 생성 
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        //상태를 Burned로 변경
                        state = State.Burned;

                        //상태 변경 이벤트 호출
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs 
                        { 
                            state = state 
                        });

                        //진행 상황 초기화
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });

                    }
                    break;
                case State.Burned:
                    break; 
            }
            Debug.Log(state);
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
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    //Frying 레시피 설정 및 상태를 Frying로 변경 
                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Frying;
                    fryingTimer = 0f;

                    //상태 변경 이벤트 호출 
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });

                    //요리 진행 상황 변경 이벤트 호출
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    }); 
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
                        GetKitchenObject().DestroySelf();

                        state = State.Idle;

                        //상태 변경 이벤트 호출
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        //진행 상황 초기화
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else
            {
                //플레이어가 아무것도 가지고 있지 않은 경우
                GetKitchenObject().SetKitchenObjectParent(player);

                state = State.Idle;

                //상태 변경 이벤트 호출
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });

                //진행 상황 초기화
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }

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
        return state == State.Fried;
    }
}
