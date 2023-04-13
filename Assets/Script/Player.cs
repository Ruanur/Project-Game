using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField]private float moveSpeed = 7f; //Unity 내에서 속도 변환이 가능하게 해줌 > public 
    private bool isWalking;
    private void Update()
    {
        Vector2 inputVector = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y = +1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = +1; 
        }

        Vector3 Dir = new Vector3(inputVector.x, 0f, inputVector.y);
        transform.position += Dir* moveSpeed * Time.deltaTime;

        isWalking = Dir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, Dir, Time.deltaTime * rotateSpeed);

    }
    public bool IsWalking()
    {
        return isWalking;
    }
    }

