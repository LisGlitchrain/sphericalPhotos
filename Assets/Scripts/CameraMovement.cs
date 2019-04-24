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
    [SerializeField] float touchFloatingResistance = 0.05f;
    [Range(1f,100f)]
    [SerializeField] float touchDivider = 5f;
    float currentMagneticAngle = 0f;
    KalmanFilterSimple1D yawKalman;
    KalmanFilterSimple1D pitchKalman;
    KalmanFilterSimple1D rollKalman;
    [SerializeField] float fy = 1;
    [SerializeField] float hy = 1;
    [SerializeField] float qy = 0.05f;
    [SerializeField] float ry = 15;
    [SerializeField] float covariance = 0.1f;
    [SerializeField] float fp = 1;
    [SerializeField] float hp = 1;
    [SerializeField] float qp = 2;
    [SerializeField] float rp = 15;
    [SerializeField] float fr = 1;
    [SerializeField] float hr = 1;
    [SerializeField] float qr = 1;
    [SerializeField] float rr = 15;
    // Start is called before the first frame update
    void Start()
    {
        yawKalman = new KalmanFilterSimple1D(qy,ry,fy,hy);
        pitchKalman = new KalmanFilterSimple1D(qp, rp, fp, hp);
        rollKalman = new KalmanFilterSimple1D(qr, rr, fr, hr);
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
        fy = (float) yawKalman.F;
        hy = (float) yawKalman.H;
        qy = (float) yawKalman.Q;
        ry = (float) yawKalman.R;
        covariance = (float) yawKalman.Covariance;

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
                    xAngularSpeed = (touch.deltaPosition.y / touchDivider ) / Time.deltaTime;
                    yAngularSpeed = (-touch.deltaPosition.x / touchDivider) / Time.deltaTime;
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
            angularSpeed = Mathf.Lerp(angularSpeed, 0, touchFloatingResistance);
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
        yawKalman.SetState(Input.compass.magneticHeading, covariance);
        currentMagneticAngle = (float) yawKalman.State;
        if (Input.acceleration.z > 0)
            pitchKalman.SetState(Mathf.Asin(1f + Input.acceleration.y) * 180 / Mathf.PI, covariance);
        else pitchKalman.SetState(Mathf.Asin(-(1f + Input.acceleration.y)) * 180 / Mathf.PI, covariance);
        rollKalman.SetState(-Input.acceleration.x * 90f, covariance);
    }

    //Something is wrong here.
    void AccelMangetometerModifyCamera()
    {
        if (Input.acceleration.z < 0)
            pitchKalman.Correct(Mathf.Asin(1f + Input.acceleration.y) * 180 / Mathf.PI);
        else
            pitchKalman.Correct(Mathf.Asin(-(1f + Input.acceleration.y)) * 180 / Mathf.PI);
        transform.rotation = Quaternion.AngleAxis((float)pitchKalman.State, Vector3.right);
        rollKalman.Correct(-Input.acceleration.x * 90f);
        transform.RotateAround(transform.position, transform.forward, (float) rollKalman.State);
        yawKalman.Correct(Input.compass.magneticHeading);
        transform.RotateAround(transform.position, Vector3.up, (float)yawKalman.State );
    }
}
