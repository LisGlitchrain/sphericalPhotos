using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Window
{
    Magnetic,
    Spherical,
    VR
}

public class Back : MonoBehaviour
{


    [SerializeField] GameObject vrCamera;
    [SerializeField] GameObject sphericalCamera;
    public Window currentWindow { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (currentWindow)
            {
                case Window.VR:
                    vrCamera.SetActive(false);
                    sphericalCamera.SetActive(true);
                    break;
                case Window.Spherical:
                    break;

            }

        }            
    }
        
}
