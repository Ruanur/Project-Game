using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    // ��ᰡ �߰��� �� �߻��ϴ� �̺�Ʈ ����
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

    // ��� �߰� �̺�Ʈ �μ� Ŭ���� ����
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    // ��ȿ�� ��� ����� �����ϴ� ����Ʈ
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    // ���� ���ÿ� �߰��� ������ �����ϴ� ����Ʈ
    private List<KitchenObjectSO> kitchenObjectSOList;

    // Awake �޼���: �ʱ� ���� ����
    protected override void Awake()
    {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    // ��Ḧ �߰��Ϸ� �õ��ϴ� �޼���
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        // ��ȿ�� ������� Ȯ��
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // ��ȿ���� ���� ���
            return false;
        }

        // �̹� �߰��� ������� Ȯ��
        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // �̹� �߰��� ���
            return false;
        }
        else
        {
            // ��Ḧ ������ �߰�
            AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
            return true;
        }
    }

    // ���� ������ ��Ḧ �߰��ϴ� �޼��� (���� RPC)
    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        // Ŭ���̾�Ʈ ������ ��� �߰� �޼��� ȣ��
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    // Ŭ���̾�Ʈ ������ ��Ḧ �߰��ϴ� �޼��� (Ŭ���̾�Ʈ RPC)
    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        // �ε����� ���� ��� ���� ��������
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        // ��Ḧ ����Ʈ�� �߰�
        kitchenObjectSOList.Add(kitchenObjectSO);

        // ��ᰡ �߰��Ǿ����� �˸��� �̺�Ʈ �߻�
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }

    // ���� ���ÿ� �߰��� ��� ����� ����� ��ȯ�ϴ� �޼���
    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
}
