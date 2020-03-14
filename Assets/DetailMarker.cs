using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailMarker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }

    public void OnChangeCamera(Transform targetTransform)
    {
        SceneMgr.GetInstance.ChangeCameraToTargetPoint(targetTransform);
        SceneMgr.GetInstance.ChangeModelToTargetScale(Vector3.one);
        SceneMgr.GetInstance.ChangeModelToTargetRotation(Vector3.zero,Vector3.zero);
        SceneMgr.GetInstance._threeDUI.SetActive(false);
        SceneMgr.GetInstance._isDetail = true;
        SceneMgr.GetInstance._currentDetailPoint = targetTransform;
    }
}
