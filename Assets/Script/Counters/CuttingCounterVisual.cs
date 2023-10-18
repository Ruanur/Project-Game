using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    //애니메이터의 트리거 이름을 정의
    private const string CUT = "Cut";

    [SerializeField] private CuttingCounter cuttingCounter;
    private Animator animator;
    
    
    private void Awake()
    {
        //컴포넌트로부터 애니메이터를 가져와 초기화 함
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cuttingCounter.OnCut += CuttingCounter_OnCut;
    }

    //CuttingCounter의 OnCut 이벤트 핸들러
    private void CuttingCounter_OnCut(object sender, System.EventArgs e)
    {
        //애니메이터에 CUT 트리거를 설정하여 애니메이션을 재생한다.
        animator.SetTrigger(CUT);
    }
} 
