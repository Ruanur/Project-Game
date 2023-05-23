using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconsSingleUI : MonoBehaviour
{
    [SerializeField] private Image image;
    public void SetKichenObjectSO(KitchenObjectSO kitchenObejctSO) {
        image.sprite = kitchenObejctSO.sprite;


    }
}
