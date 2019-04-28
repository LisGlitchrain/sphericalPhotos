using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoManager : MonoBehaviour
{
    [SerializeField] string pathTo3DPhotos = string.Empty;
    [SerializeField] Camera sphericalCamera;
    [SerializeField] Camera magneticScrollCamera;
    public Material defaultSkyBox;
    public List<Material> skyboxPhotos = new List<Material>();

    int photoSkyboxIndex = 0;
    bool isSpherical;

    // Start is called before the first frame update
    void Start()
    {
        Material[] resources = Resources.LoadAll<Material>(pathTo3DPhotos);
        foreach(var mat in resources)
        {
            skyboxPhotos.Add(mat);
        }

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangePhotoType()
    {
        if (isSpherical)
        {
            magneticScrollCamera.gameObject.SetActive(true);
            sphericalCamera.gameObject.SetActive(false);
            RenderSettings.skybox = defaultSkyBox;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.orientation = ScreenOrientation.AutoRotation;
            HideAllRenderers(false);
        }
        else if (skyboxPhotos.Count>0)
        {
            magneticScrollCamera.gameObject.SetActive(false);
            sphericalCamera.gameObject.SetActive(true);
            RenderSettings.skybox = skyboxPhotos[photoSkyboxIndex];
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.orientation = ScreenOrientation.AutoRotation;
            HideAllRenderers(true);       
        }
        isSpherical = !isSpherical;
    }

    void HideAllRenderers(bool hide)
    {
        foreach (var renderer in FindObjectsOfType<Renderer>())
            renderer.enabled = !hide;
    }

    public void Next3Dphoto()
    {
        if (skyboxPhotos.Count > photoSkyboxIndex + 1)
            RenderSettings.skybox = skyboxPhotos[++photoSkyboxIndex];
        else print("There are no more photos.");
    }

    public void Prev3DPhoto()
    {
        if (photoSkyboxIndex>0)
            RenderSettings.skybox = skyboxPhotos[--photoSkyboxIndex];
        else print("There are no more photos.");
    }
}
