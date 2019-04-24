using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Drawing;
using System.IO;

public class PhotoLoader : MonoBehaviour
{
    Transform viewport;
    [SerializeField] string pathToPhotos;
    [SerializeField] string pathToPanel;
    Sprite[] raws;

    GameObject panel;
    // Start is called before the first frame update
    public void Start()
    {
        var obj = FindObjectOfType<CanvasGroup>().gameObject;
        if (obj.name == "Viewport")
            viewport = obj.transform;
        raws = Resources.LoadAll<Sprite>(@"Photos\");
        panel = Resources.Load<GameObject>(@"Panel\Panel");

        foreach(var raw in raws)
        {
            GameObject localPanel = Instantiate(panel, viewport);          
            localPanel.AddComponent<Image>();
            localPanel.GetComponent<Image>().sprite = raw;
            //print($"file: {raw.name} x:{raw.rect.width} y:{raw.rect.height}");
            SetCorrectSizeToPanel(localPanel.GetComponent<RectTransform>(), obj.GetComponent<RectTransform>(), raw);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector2 ResolutionOfTexture(Sprite texture)
    {
        return new Vector2(texture.rect.width, texture.rect.height);
    }

    Vector2 CorrectedResolutionOfTexture(RectTransform view, Vector2 textureResolution)
    {
        Vector2 newResolution = new Vector2(textureResolution.x, textureResolution.y);
        if (view.rect.width < newResolution.x)
            newResolution = newResolution / (newResolution.x / view.rect.width);
        if (view.rect.height < newResolution.y)
            newResolution = newResolution / (newResolution.y / view.rect.height);
        return newResolution;
    }

    void SetCorrectSizeToPanel(RectTransform panel, RectTransform view, Sprite texture)
    {
        Vector2 correctSizeFloat = ResolutionOfTexture(texture);
        correctSizeFloat = CorrectedResolutionOfTexture(view, correctSizeFloat);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, correctSizeFloat.x);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, correctSizeFloat.y);
    }

    //Vector2[] GetOriginalResolutions()
    //{
    //    print($"{Application.dataPath}");
    //    DirectoryInfo d = new DirectoryInfo(Application.dataPath +@"\Resources\Photos");//Assuming Test is your Folder
    //    FileInfo[] files = d.GetFiles("*.jpg"); //Getting Text files
    //    List<Vector2> resolutions = new List<Vector2>();
    //    foreach (FileInfo file in files)
    //    {
    //        print($"{file.Name}");
    //        image = Bitmap.FromFile(Application.dataPath+@"\Resources\Photos"+file.Name);
    //        resolutions.Add(new Vector2(image.Width,image.Height));
    //    }
    //    return resolutions.ToArray();
    //}

}
