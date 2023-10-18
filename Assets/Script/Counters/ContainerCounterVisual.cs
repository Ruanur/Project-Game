using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    //애니메이터의 트리거 이름
    private const string OPEN_CLOSE = "OpenClose";

    [SerializeField] private ContainerCounter containerCounter;
    private Animator animator;

    //객체가 생성될 때 호출되는 Awake
    private void Awake()
    {
        //컴포넌트로부터 애니메이터를 가져와 초기화
        animator = GetComponent<Animator>();
    }

    //객체가 활성화 된 후 호출되는 Start 메서드 / Awake -> Start
    private void Start()
    {
        //ContainerCounter의 OnPlayerGrabbedObject 이벤트에 이벤트 핸들러 추가
        containerCounter.OnPlayerGrabbedObject += ContainerCounter_OnPlayerGrabbedObject;
    }

    //ContainerCounter의 OnPlayerGrabbedObject 이벤트 핸들러
    private void ContainerCounter_OnPlayerGrabbedObject(object sender, System.EventArgs e)
    {
        //애니메이터에 OPEN_CLOSE 트리거를 설정하여 애니메이션 재생
        animator.SetTrigger(OPEN_CLOSE);
    }
} 
