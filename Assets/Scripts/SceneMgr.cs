using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneMgr : MonoBehaviour
{
    private static SceneMgr instance;
    public static SceneMgr GetInstance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<SceneMgr>();
                if(instance == null)
                {
                    GameObject go = new GameObject("SceneMgr");
                    instance = go.AddComponent<SceneMgr>();
                }
            }
            return instance;
        }
    }

    public Transform _modelX;
    public Transform _modelY;
    public Transform _modelZ;
    public Transform _model;
    public GameObject _threeDUI;
    public AudioSource _bgm;

    Transform _cameraTransform;
    Vector3 _cameraCurrentPos;
    Quaternion _cameraCurrentQua;
    Vector3 _cameraTargetPos;
    Quaternion _cameraTargetQua;
    public bool _isCameraMove = false;
    float _moveTime = 0;
    public float _moveSpeed = 1;

    Vector3 _modelXCurrentEulerAngle;
    Vector3 _modelYCurrentEulerAngle;
    public bool _isModelCanRotate = false;
    float _rotateTime = 0;

    private Vector2 _currOnePos;
    private Vector2 _lastOnePos;
    public float _rotateSpeed = 1;

    private float _currTwoDistance;
    private float _lastTwoDistance;
    public float _changeSpeed = 1;

    public bool _isDetail = false;
    Vector3 _modelCurrentScale;
    float _scaleTime = 0;

    public Transform _currentDetailPoint;

    enum FingerCount
    {
        zero,
        one,
        two,
        threeOrAbove
    }

    FingerCount _fingerCount = FingerCount.zero;

    // Use this for initialization
    void Start()
    {
        _cameraTransform = Camera.main.gameObject.transform;
        _modelX.rotation = Quaternion.identity;
        _modelY.rotation = Quaternion.identity;
        _modelZ.rotation = Quaternion.identity;
        _model.localScale = Vector3.one;
        _threeDUI.SetActive(false);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(_isCameraMove)
        {
            _moveTime += Time.deltaTime;
            _cameraTransform.position = Vector3.Lerp(_cameraCurrentPos,_cameraTargetPos,_moveTime * _moveSpeed);
            _cameraTransform.rotation = Quaternion.Lerp(_cameraCurrentQua,_cameraTargetQua,_moveTime * _moveSpeed);
            if(_moveTime > (1 / _moveSpeed))
            {
                _moveTime = 0;
                _isCameraMove = false;
                _cameraTransform.position = _cameraTargetPos;
                _cameraTransform.rotation = _cameraTargetQua;
            }
        }

    }

    public void ChangeCameraToTargetPoint(Transform targetTransform)
    {
        if(_isCameraMove)
            return;
        _isCameraMove = true;
        _cameraCurrentPos = _cameraTransform.position;
        _cameraCurrentQua = _cameraTransform.rotation;
        _cameraTargetPos = targetTransform.position;
        _cameraTargetQua = targetTransform.rotation;
    }

    public void ChangeModelToTargetRotation(Vector3 modelXTargetEulerAngle,Vector3 modelYTargetEulerAngle)
    {
        StartCoroutine(ChangeModelToTargetRotationAsync(modelXTargetEulerAngle,modelYTargetEulerAngle));
    }

    public void ChangeModelToTargetScale(Vector3 modelTargetScale)
    {
        StartCoroutine(ChangeModelToTargetScaleAsync(modelTargetScale));
    }

    IEnumerator ChangeModelToTargetScaleAsync(Vector3 modelTargetScale)
    {
        _modelCurrentScale = _model.localScale;
        while(true)
        {
            _scaleTime += Time.deltaTime;
            _model.localScale = Vector3.Lerp(_modelCurrentScale,modelTargetScale,_scaleTime);
            if(_scaleTime >= 1)
            {
                _model.localScale = modelTargetScale;
                _scaleTime = 0;
                break;
            }
            yield return null;
        }
    }

    IEnumerator ChangeModelToTargetRotationAsync(Vector3 modelXTargetEulerAngle,Vector3 modelYTargetEulerAngle)
    {
        _modelXCurrentEulerAngle = _modelX.localEulerAngles;
        _modelYCurrentEulerAngle = _modelY.localEulerAngles;
        if(_modelXCurrentEulerAngle.x >= 180)
        {
            _modelXCurrentEulerAngle -= new Vector3(360f,0,0);
        }

        if(_modelYCurrentEulerAngle.y >= 180)
        {
            _modelYCurrentEulerAngle -= new Vector3(0,360f,0);
        }
        _isModelCanRotate = false;
        while(true)
        {
            _rotateTime += Time.deltaTime;
            _modelX.localEulerAngles = Vector3.Lerp(_modelXCurrentEulerAngle,modelXTargetEulerAngle,_rotateTime);
            _modelY.localEulerAngles = Vector3.Lerp(_modelYCurrentEulerAngle,modelYTargetEulerAngle,_rotateTime);
            if(_rotateTime > 1f)
            {

                _isModelCanRotate = true;
                _modelX.localEulerAngles = modelXTargetEulerAngle;
                _modelY.localEulerAngles = modelYTargetEulerAngle;
                _rotateTime = 0;
                break;
            }
            yield return null;
        }
    }

    private void Update()
    {
        if(!_isDetail)
        {
            if(_isModelCanRotate)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    _lastOnePos = Input.mousePosition;
                    return;
                }
                if(Input.GetMouseButton(0))
                {
                    _currOnePos = Input.mousePosition;
                    Vector2 delta = (_currOnePos - _lastOnePos) * _rotateSpeed * Time.deltaTime;
                    _modelX.Rotate(new Vector3(delta.y,0,0));
                    _modelY.Rotate(new Vector3(0,-delta.x,0));


                    if(Vector3.Cross(_modelX.up,Vector3.up).x <= 0)
                    {
                        _modelX.localRotation = Quaternion.Euler(0,0,0);
                    }
                    else
                    {
                        if(Vector3.Angle(_modelX.up,Vector3.up) > 90f)
                        {
                            _modelX.localRotation = Quaternion.Euler(270f,0,0);
                        }
                    }

                    if(Vector3.Cross(_modelY.forward,Vector3.forward).y >= 0.7f)
                    {
                        _modelY.localRotation = Quaternion.Euler(0,-45f,0);
                    }
                    if(Vector3.Cross(_modelY.forward,Vector3.forward).y <= -0.7f)
                    {
                        _modelY.localRotation = Quaternion.Euler(0,45f,0);
                    }

                    _lastOnePos = _currOnePos;
                }

                if(Input.touchCount == 0)
                {
                    _fingerCount = FingerCount.zero;
                }
                if(Input.touchCount == 1)
                {

                    if(_fingerCount != FingerCount.one)
                    {
                        _lastOnePos = Input.GetTouch(0).position;
                        _fingerCount = FingerCount.one;
                        return;
                    }

                    if(Input.GetTouch(0).phase == TouchPhase.Moved && _fingerCount == FingerCount.one)
                    {
                        _currOnePos = Input.GetTouch(0).position;
                        Vector2 delta = (_currOnePos - _lastOnePos) * _rotateSpeed * Time.deltaTime;
                        _modelX.Rotate(new Vector3(delta.y,0,0));
                        _modelY.Rotate(new Vector3(0,-delta.x,0));

                        if(Vector3.Cross(_modelX.up,Vector3.up).x <= 0)
                        {
                            _modelX.localRotation = Quaternion.Euler(0,0,0);
                        }
                        else
                        {
                            if(Vector3.Angle(_modelX.up,Vector3.up) > 90f)
                            {
                                _modelX.localRotation = Quaternion.Euler(270f,0,0);
                            }
                        }

                        if(Vector3.Cross(_modelY.forward,Vector3.forward).y >= 0.7f)
                        {
                            _modelY.localRotation = Quaternion.Euler(0,-45f,0);
                        }
                        if(Vector3.Cross(_modelY.forward,Vector3.forward).y <= -0.7f)
                        {
                            _modelY.localRotation = Quaternion.Euler(0,45f,0);
                        }
                        _lastOnePos = _currOnePos;
                    }

                }
                if(Input.touchCount == 2)
                {
                    if(_fingerCount != FingerCount.two)
                    {
                        _lastTwoDistance = Vector2.Distance(Input.GetTouch(0).position,Input.GetTouch(1).position);
                        _fingerCount = FingerCount.two;
                        return;
                    }
                    if(_fingerCount == FingerCount.two)
                    {
                        if(Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                        {
                            _currTwoDistance = Vector2.Distance(Input.GetTouch(0).position,Input.GetTouch(1).position);
                            float delta = _currTwoDistance - _lastTwoDistance;
                            _model.localScale += Vector3.one * delta * Time.deltaTime * _changeSpeed;
                            if(_model.localScale.x >= 2)
                            {
                                _model.localScale = Vector3.one * 2f;
                            }
                            if(_model.localScale.x <= 0.5f)
                            {
                                _model.localScale = Vector3.one * 0.5f;
                            }
                            _lastTwoDistance = _currTwoDistance;

                        }
                    }

                    if(_fingerCount == FingerCount.threeOrAbove)
                    {
                        return;
                    }

                }
                if(Input.touchCount >= 3)
                {
                    _fingerCount = FingerCount.threeOrAbove;
                }
            }
        }
        else
        {
            if(Input.GetMouseButtonDown(0))
            {
                _lastOnePos = Input.mousePosition;
                return;
            }
            if(Input.GetMouseButton(0))
            {
                _currOnePos = Input.mousePosition;
                Vector2 delta = (_currOnePos - _lastOnePos) * _rotateSpeed * Time.deltaTime;
                _cameraTransform.Rotate(_currentDetailPoint.up,delta.x,Space.World);
                _cameraTransform.Rotate(_currentDetailPoint.right,-delta.y,Space.World);
                _cameraTransform.rotation = Quaternion.Euler(_cameraTransform.localEulerAngles.x,_cameraTransform.localEulerAngles.y,0);

                if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).x >= 0.7f)
                {
                    _cameraTransform.rotation = Quaternion.Euler(-45f,_cameraTransform.localEulerAngles.y,0);
                }
                if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).x <= -0.7f)
                {
                    _cameraTransform.rotation = Quaternion.Euler(45f,_cameraTransform.localEulerAngles.y,0);
                }

                if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).z >= 0.7f)
                {
                    _cameraTransform.rotation = Quaternion.Euler(-45f,_cameraTransform.localEulerAngles.y,0);
                }
                if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).z <= -0.7f)
                {
                    _cameraTransform.rotation = Quaternion.Euler(45f,_cameraTransform.localEulerAngles.y,0);
                }


                if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).y >= 0.7f)
                {
                    _cameraTransform.localRotation = Quaternion.Euler(_cameraTransform.localEulerAngles.x,-45f - Vector3.Angle(_currentDetailPoint.forward,Vector3.forward),0);
                }
                if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).y <= -0.7f)
                {
                    _cameraTransform.localRotation = Quaternion.Euler(_cameraTransform.localEulerAngles.x,45f - Vector3.Angle(_currentDetailPoint.forward,Vector3.forward),0);
                }

                _lastOnePos = _currOnePos;
            }

            if(Input.touchCount == 0)
            {
                _fingerCount = FingerCount.zero;
            }
            if(Input.touchCount == 1)
            {

                if(_fingerCount != FingerCount.one)
                {
                    _lastOnePos = Input.GetTouch(0).position;
                    _fingerCount = FingerCount.one;
                    return;
                }

                if(Input.GetTouch(0).phase == TouchPhase.Moved && _fingerCount == FingerCount.one)
                {
                    _currOnePos = Input.GetTouch(0).position;
                    Vector2 delta = (_currOnePos - _lastOnePos) * _rotateSpeed * Time.deltaTime;
                    _cameraTransform.Rotate(_currentDetailPoint.up,delta.x,Space.World);
                    _cameraTransform.Rotate(_currentDetailPoint.right,-delta.y,Space.World);
                    _cameraTransform.rotation = Quaternion.Euler(_cameraTransform.localEulerAngles.x,_cameraTransform.localEulerAngles.y,0);

                    if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).x >= 0.7f)
                    {
                        _cameraTransform.rotation = Quaternion.Euler(-45f,_cameraTransform.localEulerAngles.y,0);
                    }
                    if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).x <= -0.7f)
                    {
                        _cameraTransform.rotation = Quaternion.Euler(45f,_cameraTransform.localEulerAngles.y,0);
                    }

                    if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).z >= 0.7f)
                    {
                        _cameraTransform.rotation = Quaternion.Euler(-45f,_cameraTransform.localEulerAngles.y,0);
                    }
                    if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).z <= -0.7f)
                    {
                        _cameraTransform.rotation = Quaternion.Euler(45f,_cameraTransform.localEulerAngles.y,0);
                    }


                    if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).y >= 0.7f)
                    {
                        _cameraTransform.localRotation = Quaternion.Euler(_cameraTransform.localEulerAngles.x,-45f - Vector3.Angle(_currentDetailPoint.forward,Vector3.forward),0);
                    }
                    if(Vector3.Cross(_cameraTransform.forward,_currentDetailPoint.forward).y <= -0.7f)
                    {
                        _cameraTransform.localRotation = Quaternion.Euler(_cameraTransform.localEulerAngles.x,45f - Vector3.Angle(_currentDetailPoint.forward,Vector3.forward),0);
                    }
                    _lastOnePos = _currOnePos;
                }

            }
            if(Input.touchCount == 2)
            {
                _fingerCount = FingerCount.two;
            }
            if(Input.touchCount >= 3)
            {
                _fingerCount = FingerCount.threeOrAbove;
            }
        }

    }
}
