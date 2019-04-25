using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoManager : MonoBehaviour
{
    bool isSpherical;

    [SerializeField] string pathTo3DPhotos = string.Empty;

    public Material defaultSkyBox;
    int photoSkyboxIndex = 0;
    public List<Material> skyboxPhotos = new List<Material>();

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
            RenderSettings.skybox = defaultSkyBox;
            HideAllRenderers(false);
        }
        else if (skyboxPhotos.Count>0)
        {
            RenderSettings.skybox = skyboxPhotos[photoSkyboxIndex];
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
