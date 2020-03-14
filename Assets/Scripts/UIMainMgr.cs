using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMgr : MonoBehaviour
{
    public GameObject _enter;
    public GameObject _mainUI;
    public GameObject _detailContents;

    // Use this for initialization
    void Start()
    {
        _enter.SetActive(true);
        _mainUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnEnterButton()
    {
        SceneMgr.GetInstance.ChangeModelToTargetRotation(new Vector3(-22.5f,0,0),new Vector3(0,22.5f,0));
        _enter.SetActive(false);
        StartCoroutine(ShowTargetAsync(_mainUI));
        StartCoroutine(ShowTargetAsync(SceneMgr.GetInstance._threeDUI));
        SceneMgr.GetInstance._isModelCanRotate = true;
    }

    public void OnIntroductionButton(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }

    public void OnUpButton()
    {
        if(SceneMgr.GetInstance._isModelCanRotate)
        {
            SceneMgr.GetInstance.ChangeModelToTargetRotation(new Vector3(-90f,0,0),new Vector3(0,0,0));
            StartCoroutine(ShowTargetAsync(SceneMgr.GetInstance._threeDUI));
            SceneMgr.GetInstance._isDetail = false;
            CloseAllDetailContent();
        }

    }

    public void OnFrontButton()
    {
        if(SceneMgr.GetInstance._isModelCanRotate)
        {
            SceneMgr.GetInstance.ChangeModelToTargetRotation(new Vector3(-22.5f,0,0),new Vector3(0,22.5f,0));
            StartCoroutine(ShowTargetAsync(SceneMgr.GetInstance._threeDUI));
            SceneMgr.GetInstance._isDetail = false;
            CloseAllDetailContent();
        }

    }

    public void OnBGMButton(Transform image)
    {
        BGMRotate bgmRotate = image.GetComponent<BGMRotate>();
        if(bgmRotate._isRotate)
        {
            bgmRotate._isRotate = false;
            SceneMgr.GetInstance._bgm.Pause();
        }
        else
        {
            bgmRotate._isRotate = true;
            SceneMgr.GetInstance._bgm.Play();
        }
    }

    public void ShowDetailContent(GameObject targetDetail)
    {
        StartCoroutine(ShowTargetAsync(targetDetail));
    }

    IEnumerator ShowTargetAsync(GameObject target)
    {
        yield return new WaitForSeconds(1f);
        target.SetActive(true);
    }

    void CloseAllDetailContent()
    {
        for(int i = 0;i < _detailContents.transform.childCount;i++)
        {
            _detailContents.transform.GetChild(i).Find("Image").gameObject.SetActive(false);
            _detailContents.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ShowContentPic(GameObject targetPic)
    {
        targetPic.SetActive(true);
    }
}
