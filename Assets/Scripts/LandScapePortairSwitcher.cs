using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandScapePortairSwitcher : MonoBehaviour
{
    DeviceOrientation prevOrientation = DeviceOrientation.LandscapeLeft;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print($"{Input.deviceOrientation}");
        if(Input.deviceOrientation != prevOrientation)
        {
            Camera.main.aspect = 1 / Camera.main.aspect;
            DestroyImmediate(FindObjectOfType<MagneticScrollView.MagneticScrollRect>());
            GameObject obj = FindObjectOfType<CanvasGroup>().gameObject;
            for(var i= obj.transform.childCount-1; i>=0;i--)
            {
                //DestroyImmediate(obj.transform.GetChild(i).GetComponent<Image>());
                DestroyImmediate(obj.transform.GetChild(i).gameObject);
            }
            FindObjectOfType<PhotoLoader>().Start();
            GameObject scrollView = GameObject.Find("Magnetic Scroll View");
            scrollView.AddComponent<MagneticScrollView.MagneticScrollRect>();
            scrollView.GetComponent<MagneticScrollView.MagneticScrollRect>().viewport = scrollView.transform.GetChild(0).GetComponent<RectTransform>();
            scrollView.GetComponent<MagneticScrollView.MagneticScrollRect>().ResizeModeEnum = MagneticScrollView.ResizeMode.Free;
            prevOrientation = Input.deviceOrientation;
        }
    }
}
