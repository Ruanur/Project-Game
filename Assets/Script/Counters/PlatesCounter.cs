using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    //접시가 생성될 때 호출되는 이벤트
    public event EventHandler OnPlateSpawned;

    //접시가 제거될 때 호출되는 이벤트 
    public event EventHandler OnPlateRemoved;


    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;


    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    //매 프레임 업데이트되는 메소드 
    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;

            if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax)
            {
                //게임이 진행 중이며 생성된 접시 수가 제한(4)에 걸리지 않을 때 접시를 생성함
                SpawnPlateServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawnedAmount++;

        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    //플레이어와 상호작용하는 메소드 재정의
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //플레이어가 손에 아무것도 들고 있지 않을 때
            if (platesSpawnedAmount > 0)
            {
                //적어도 하나의 접시가 있을 때, 플레이어에게 접시를 전달한다. 
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                InteractLogicServerRpc();
            }
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
        platesSpawnedAmount--;

        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }

}