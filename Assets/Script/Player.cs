using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class Player : NetworkBehaviour, IKitchenObjectParent
{

    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }

    public static Player LocalInstance { get; private set; }


    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }


    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private PlayerVisual playerVisual;


    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    //���� �޼ҵ�: �ʱ� ���� ����
    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

        //�÷��̾� �����͸� ������ ������ ����
        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    }

    //��Ʈ��ũ ���� �޼ҵ� : ��Ʈ��ũ �󿡼� ������ �� ȣ���
    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if(IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnetCallback;
        }
    }

    //Ŭ���̾�Ʈ ���� ���� �ݹ� : Ŭ���̾�Ʈ�� ���� ������ �� ȣ��� 
    private void NetworkManager_OnClientDisconnetCallback(ulong clientId)
    {
        //Ŭ���̾�Ʈ�� �ֹ� ������Ʈ�� ������ �ִ� ��쿡
        if(clientId == OwnerClientId && HasKitchenObject())
        {
            //������Ʈ�� �ı�
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    //���� ��ȣ�ۿ� �Է� ó�� �޼ҵ�
    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        //������ �������� �ƴ϶�� �ʱ�ȭ
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        //���õ� ī���Ͱ� �ִ� ���
        if (selectedCounter != null)
        {
            //����
            selectedCounter.InteractAlternate(this);
        }
    }

    //��ȣ�ۿ� �Է� ó�� �޼ҵ�
    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        //������ �������� �ƴ϶�� ����
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        //���õ� ī���Ͱ� �ִ� ���
        if (selectedCounter != null)
        {
            //��ȣ�ۿ� ����
            selectedCounter.Interact(this);
        }
    }

    //������Ʈ �޼ҵ� : �� ������ ���� ȣ��
    private void Update()
    {
        if(!IsOwner)
        {
            return;
        } 
        HandleMovement(); //�̵� ó�� �Լ� ȣ��
        HandleInteractions(); //��ȣ�ۿ� ó�� �Լ� ȣ��
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    //��ȣ �ۿ� ó�� �޼ҵ�
    private void HandleInteractions()
    {
        //�Է� ���� ��������
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        //�̵� ���� ����
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f; //��ȣ �ۿ� �Ÿ�
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // Has ClearCounter
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);

            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    //�̵� ó�� �޼ҵ�
    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        //�̵� �Ÿ� ���
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        //float playerHeight = 2f;
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, collisionsLayerMask);



        if (!canMove)
        {
            // Cannot move towards moveDir

            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);

            if (canMove)
            {
                // Can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);

                if (canMove)
                {
                    // Can move only on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        //�÷��̾� ȸ�� ������ �̵� �������� ����, �Ӹ� ���ư���
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if(kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

}