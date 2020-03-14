using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMRotate : MonoBehaviour
{
    public bool _isRotate = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(_isRotate)
            transform.eulerAngles -= new Vector3(0,0,100f)*Time.deltaTime;
    }
}
