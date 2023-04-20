using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] private Transform tomatoPrefab;
    [SerializeField] private Transform CounterTopPoint;
    public void Interact()
    {
        Debug.Log("Interact!");
        Transform tomatoTransform = Instantiate(tomatoPrefab, CounterTopPoint);
        tomatoTransform.localPosition = Vector3.zero;
    }
}
