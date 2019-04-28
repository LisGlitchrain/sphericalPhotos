using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandScapePortairSwitcher : MonoBehaviour
{
    [SerializeField] Camera magneticCamera;

    DeviceOrientation prevOrientation = DeviceOrientation.LandscapeLeft;
    bool orientationChangedInPreviousFrame = false;
    GameObject viewport;
    int photoIndex;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<PhotoLoader>().Load();
    }

    // Update is called once per frame
    void Update()
    {
        if (orientationChangedInPreviousFrame)
        {
            viewport.GetComponent<RectTransform>().ForceUpdateRectTransforms();
            FindObjectOfType<MagneticScrollView.MagneticScrollRect>().ChangeOrientation();      
            FindObjectOfType<MagneticScrollView.MagneticScrollRect>().Update();
            FindObjectOfType<PhotoLoader>().Load();
            //print($"photo n {photoIndex}");
            //FindObjectOfType<MagneticScrollView.MagneticScrollRect>().ForceCurrentSelectedIndexSet(photoIndex);
            orientationChangedInPreviousFrame = false;
        }
        if (Input.deviceOrientation!= DeviceOrientation.FaceUp && 
            Input.deviceOrientation != DeviceOrientation.FaceDown &&
            Input.deviceOrientation != DeviceOrientation.Unknown)
            if (Input.deviceOrientation != prevOrientation) 
            {
                //photoIndex = FindObjectOfType<MagneticScrollView.MagneticScrollRect>().CurrentSelectedIndex;
                magneticCamera.ResetAspect();
                GameObject obj = FindObjectOfType<CanvasGroup>().gameObject;
                if (obj.name == "Viewport")
                    viewport = obj;
                for (var i = viewport.transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(viewport.transform.GetChild(i).gameObject);
                }
                viewport.GetComponent<RectTransform>().ForceUpdateRectTransforms();
                prevOrientation = Input.deviceOrientation;
                FindObjectOfType<MagneticScrollView.MagneticScrollRect>().ChangeOrientation();
                FindObjectOfType<MagneticScrollView.MagneticScrollRect>().Update();
                orientationChangedInPreviousFrame = true;
            }
    }
}
