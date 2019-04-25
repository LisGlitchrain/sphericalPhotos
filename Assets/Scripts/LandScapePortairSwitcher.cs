using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandScapePortairSwitcher : MonoBehaviour
{
    DeviceOrientation prevOrientation = DeviceOrientation.LandscapeLeft;
    float defaultAspectRatio;
    bool orientationChangedInPreviousFrame = false;
    GameObject viewport;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<PhotoLoader>().Load();
        defaultAspectRatio = Camera.main.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        if (orientationChangedInPreviousFrame)
        {
            FindObjectOfType<MagneticScrollView.MagneticScrollRect>().ChangeOrientation();
            FindObjectOfType<MagneticScrollView.MagneticScrollRect>().Update();
            FindObjectOfType<PhotoLoader>().Load();
            orientationChangedInPreviousFrame = false;
        }
        if (Input.deviceOrientation != prevOrientation)
        {
            if (Input.deviceOrientation == DeviceOrientation.Portrait)
            {
                //print("Orientation has been changed!");
                Camera.main.aspect = 1 / Camera.main.aspect;                    
            }
            else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
            {
                //print("2Orientation has been changed2!");
                Camera.main.ResetAspect();
            }
            GameObject obj = FindObjectOfType<CanvasGroup>().gameObject;
            if (obj.name == "Viewport")
                viewport = obj;
            for (var i = viewport.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(viewport.transform.GetChild(i).gameObject);
            }
            viewport.GetComponent<RectTransform>().ForceUpdateRectTransforms();
            prevOrientation = Input.deviceOrientation;
            orientationChangedInPreviousFrame = true;
        }
    }
}
