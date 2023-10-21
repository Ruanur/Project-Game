using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer headMeshRenderer;
    [SerializeField] private MeshRenderer bodyMeshRenderer;

    private Material mateial;

    private void Awake()
    {
        mateial = new Material(headMeshRenderer.material);
        headMeshRenderer.material = mateial;
        bodyMeshRenderer.material = mateial;
    }

    public void SetPlayerColor(Color color)
    {
        mateial.color = color;
    }
}
