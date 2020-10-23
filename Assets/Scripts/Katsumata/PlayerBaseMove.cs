using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseMove : MonoBehaviour
{
    Rigidbody m_rb;
    [SerializeField] float forwardForce = 50;
    [SerializeField, Range(0, 1f)] public float forwardBrekeCoefficient = 0.995f;
    [SerializeField, Range(0, 1f)] public float rotateBrekeCoefficient = 0.9f;
    
    public float speed;
    public float maxSpeed = 50;
    [SerializeField] public float horizontalSpeed = 2.0f;
    Vector3 mouthPosi;

    Touch touch;
    Vector2 touchPos = new Vector2();
    Vector2 touchBeginPos;
    //Vector2 touchEndPos;
    public float swipeDistance_x = 0;
    DebugUI debugUI = new DebugUI();
    [SerializeField] GameObject debugUIobj;
    [SerializeField] bool mouthDebug;
    /// <summary>スワイプをしたかどうかのフラグ。回転力を加えるとき一回だけrotateForceに+=をしたい </summary>
    bool swipe = false;
    float rotateForce = 0;
    public float debugRotateForce = 0;
    


    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        debugUI = debugUIobj.GetComponent<DebugUI>();
    }
    // Update is called once per frame
    private void Update()
    {
        speed = m_rb.velocity.magnitude;
    }
    void FixedUpdate()
    {


        if (mouthDebug)
        {
            MouthAim();
            MouthForce();
        }
        else
        {
            TouchMoveForce();
            Swipe();
        }

        m_rb.velocity = m_rb.velocity * forwardBrekeCoefficient;
        if (m_rb.velocity.magnitude < 0.01)
        {
            m_rb.velocity = m_rb.velocity * 0;
        }


        AddRotatePlayer();
    }

    /// <summary>加減速を計算する</summary>
    void TouchMoveForce()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                m_rb.AddForce(this.transform.forward * forwardForce);
            }
        }
    }

    void MouthForce()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_rb.AddForce(this.transform.forward * forwardForce);
        }
        else if (!Input.GetMouseButton(0))
        {
            //m_rb.velocity = m_rb.velocity * coefficient;
        }
    }

    void SpeedLimiting()
    {
        //m_rb.velocity.magnitude = maxSpeed;
    }

    void Swipe()
    {

        // 画面タッチが行われたら
        if (touch.phase == TouchPhase.Began)
        {
            touchBeginPos = touch.position;
            swipeDistance_x = 0;
        }

        if (touch.phase == TouchPhase.Moved)
        {
            swipe = true;
            swipeDistance_x = touch.position.x - touchBeginPos.x; //現状デバッグ用に変数作って取得してる
            touchPos = new Vector2(
            (swipeDistance_x) / Screen.width, 0);
        }
    }
    void MouthAim()
    {
        mouthPosi = Input.mousePosition;
        touchPos.x = mouthPosi.x / Screen.width;
    }
    void AddRotatePlayer()
    {
        if (m_rb.angularVelocity.magnitude > 0)
        {
            m_rb.angularVelocity = m_rb.angularVelocity * 0;
        }
        if (swipe || mouthDebug)
        {
            rotateForce = horizontalSpeed * touchPos.x;
        }
        else
        {
            rotateForce *= rotateBrekeCoefficient;
            if (Mathf.Abs(rotateForce) <= 0.01)
            {
                rotateForce = 0;
            }
        }

        transform.Rotate(0, rotateForce, 0);
        debugRotateForce = rotateForce;
        swipe = false;

    }

    /// <summary>デバッグ用、SwipeCoefficientSliderのOnValueChangedで呼び出してる</summary>
    public void ChangeSwipeCoefficient()
    {
        horizontalSpeed = debugUI.swipeCoefficient.value;
    }

    /// <summary>これもデバッグ用、FowrardForceSliderのOnValueChangedで呼び出してる</summary>
    public void ChangeForwardForce()
    {
        forwardForce = debugUI.forwardForce.value;
    }

    /// <summary>これもデバッグ用、airBreakCoefficientSliderのOnValueChangedで呼び出してる</summary>
    public void ChangeForwardBrakeCoefficient()
    {
        forwardBrekeCoefficient = debugUI.forwardBrakeCoefficient.value;
    }
    /// <summary>これもデバッグ用、RotateBrakeCoefficientSliderのOnValueChangedで呼び出してる</summary>
    public void ChangeRotateBrakeCoefficient()
    {
        rotateBrekeCoefficient = debugUI.rotateBrakeCoefficient.value;
    }
    /// <summary>これもデバッグ用、MaxSpeedSliderのOnValueChangedで呼び出してる</summary>
    public void ChangeMaxSpeed()
    {
        maxSpeed = debugUI.maxSpeed.value;
    }
}
