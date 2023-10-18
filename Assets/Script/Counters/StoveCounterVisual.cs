using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter; //StoveCounter에 대한 참조 
    [SerializeField] private GameObject stoveOnGameObject; //화면에 보일 스토브 켜진 상태 게임 오브젝트 
    [SerializeField] private GameObject particlesGameObject; //화면에 보일 파티클 시스템 게임 오브젝트 

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        //요리 상태에 따라 시각적 요소 표시 여부 결정 
        bool showVisual = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        stoveOnGameObject.SetActive(showVisual); //스토브 켜진 상태에서 게임 오브젝트 표시 여부 결정 
        particlesGameObject.SetActive(showVisual); //파티클 시스템 게임 오브젝트 표시 여부 결정 
    }
}
