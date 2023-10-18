using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;

    private List<GameObject> plateVisualGameObjectList;

    private void Awake()
    {
        plateVisualGameObjectList = new List<GameObject>();
    }



    private void Start()
    {
        //PlatesCounter의 OnPlateSpawned 및 OnPlateRemoved 이벤트의 이벤트 핸들러 추가
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;   
    }

    // PlatesCounter의 OnPlateRemoved 이벤트 핸들러
    private void PlatesCounter_OnPlateRemoved(object sender, System.EventArgs e)
    {
        //가장 최근에 생성된 접시의 게임 오브젝트를 제거하고 리스트에서 삭제한다.
        GameObject plateGameObject = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];    
        plateVisualGameObjectList.Remove(plateGameObject);
        Destroy(plateGameObject);   
    }

    //PlatesCounter의 OnPlateSpawned 이벤트 핸들러
    private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e)
    {
        //접시 복제, y축으로 1씩 올려서 새로운 접시를 나타냄
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

        float plateOffsetY = .1f;
        plateVisualTransform.localPosition = new Vector3 (0, plateOffsetY * plateVisualGameObjectList.Count, 0);

        plateVisualGameObjectList.Add(plateVisualTransform.gameObject);
    }
}