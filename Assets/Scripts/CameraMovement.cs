using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    bool useTouchInput = true;
    [SerializeField] float yAngularSpeed = 0f;
    [SerializeField] float xAngularSpeed = 0f;
    bool touchSpeedBiggerThenThreshold;
    [Range(0,50)]
    [SerializeField] float touchThreshold = 1f;
    [Range(0,50)]
    [SerializeField] float touchFloatingResistance = 5f;
    [Range(1f,100f)]
    [SerializeField] float touchDivider = 5f;
    float currentMagneticAngle = 0f;
    [SerializeField] KalmanFilterSimple1D compasKalman;
    [SerializeField] float f = 1;
    [SerializeField] float h=1;
    [SerializeField] float q=2;
    [SerializeField] float r=15;
    [SerializeField] float covariance = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        compasKalman = new KalmanFilterSimple1D(q,r,f,h);   
    }

    // Update is called once per frame
    void Update()
    {
        if (useTouchInput)
        {
            TouchModifyCamera();
        }
        else
        {
            if ( SystemInfo.supportsGyroscope)
                GyroModifyCamera();
            else
            {
                print($"Gyroscopes are not supported on this device.");
                AccelMangetometerModifyCamera();
            }
        }
        f = (float) compasKalman.F;
        h = (float) compasKalman.H;
        q = (float) compasKalman.Q;
        r = (float) compasKalman.R;
        covariance = (float) compasKalman.Covariance;

    }

    void TouchModifyCamera()
    {
        print($"TouchCount {Input.touchCount} Bigger {touchSpeedBiggerThenThreshold} xAS {yAngularSpeed}");
        if (Input.touchCount>0)
        {
            Touch touch = Input.GetTouch(0);
            if (touchDivider != 0)
            {
                transform.RotateAround(transform.position, Vector3.up, -touch.deltaPosition.x / touchDivider);
                transform.RotateAround(transform.position, transform.right, touch.deltaPosition.y / touchDivider);
                if (touch.deltaPosition.magnitude > touchThreshold)
                {
                    touchSpeedBiggerThenThreshold = true;
                    xAngularSpeed = touch.deltaPosition.y / touchDivider;
                    yAngularSpeed = -touch.deltaPosition.x / touchDivider;
                }
                else touchSpeedBiggerThenThreshold = false;
            }
        }
        else if (touchSpeedBiggerThenThreshold)
        {
            print($"{Mathf.Sqrt(xAngularSpeed * xAngularSpeed + yAngularSpeed * yAngularSpeed)} < {0.5f}");
            transform.RotateAround(transform.position, Vector3.up, yAngularSpeed* Time.deltaTime);
            transform.RotateAround(transform.position, transform.right, xAngularSpeed * Time.deltaTime);
            xAngularSpeed = DecreaseAngularSpeed(xAngularSpeed);
            yAngularSpeed = DecreaseAngularSpeed(yAngularSpeed);
            if (Mathf.Sqrt(xAngularSpeed * xAngularSpeed + yAngularSpeed * yAngularSpeed) < 0.5f)
                touchSpeedBiggerThenThreshold = false;
        }
    }

    float DecreaseAngularSpeed(float angularSpeed)
    {
        if (Mathf.Abs(angularSpeed)> touchFloatingResistance)
        {
            angularSpeed = Mathf.Lerp(angularSpeed, 0, 0.1f);
            //if (angularSpeed>0)
            //{
            //    angularSpeed -= touchFloatingResistance * Time.deltaTime;
            //}
            //else
            //{
            //    angularSpeed += touchFloatingResistance * Time.deltaTime;
            //}
        }
        else
        {
            angularSpeed = 0f;
        }
        return angularSpeed;
    }

    void GyroModifyCamera()
    {
        transform.rotation = GyroToUnity(Input.gyro.attitude);
    }

    private Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    public void ChangeCameraMode()
    {
        useTouchInput = !useTouchInput;
        Input.compass.enabled = true;
        compasKalman.SetState(Input.compass.magneticHeading, covariance);
        currentMagneticAngle = (float) compasKalman.State;
    }

    //Something is wrong here.
    void AccelMangetometerModifyCamera()
    {
        if (Input.acceleration.z < 0)
            transform.rotation = Quaternion.AngleAxis((Input.acceleration.y +1f)* 40f, transform.right);
        else transform.rotation = Quaternion.AngleAxis(-(Input.acceleration.y+1f )* 40f, transform.right);
        compasKalman.Correct(Input.compass.magneticHeading);
        transform.RotateAround(transform.position, Vector3.up, (float)compasKalman.State );
    }
}
