using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter; //StoveCounter에 대한 참조


    private AudioSource audioSource; //오디오 소스
    private float warningSoundTimer; //경고음 재생 간격 타이머
    private bool playWarningSound; //경고음 재생 여부 플래그

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); //이 스크립트가 부착된 오브젝트의 오디오 소스 가져오기 
    }

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = .5f;
        //경고음 재생 여부 결정 : 패티가 지정된 진행 정도를 넘어설 경우 경고음 재생 
        playWarningSound = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        //상태 변경 이벤트에서 요리 상태인 경우 오디오 재생 
        bool playSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        if (playSound)
        {
            audioSource.Play(); //오디오 재생
        }
        else
        {
            audioSource.Pause(); //오디오 일시 정지
        }
    }
    private void Update()
    {
        if(playWarningSound){    
            warningSoundTimer -= Time.deltaTime;
            if (warningSoundTimer <= 0f)
            {
                float warningSoundTimerMax = .2f;
                warningSoundTimer = warningSoundTimerMax;

                //경고음 재생 
                SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
            }
        }
    }
}
