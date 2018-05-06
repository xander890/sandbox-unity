using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class ColorSourceView : MonoBehaviour
{
    public GameObject ColorSourceManager;
    private ColorSourceManager _ColorManager;
    private MeshRenderer renderer;
    
    void Start ()
    {
        renderer = GetComponent<MeshRenderer>();
    }
    
    void Update()
    {
        if (ColorSourceManager == null)
        {
            return;
        }
        
        
        if (_ColorManager == null)
        {
            _ColorManager = ColorSourceManager.GetComponent<ColorSourceManager>();
            renderer.material.mainTexture = _ColorManager.GetColorTexture();
            //renderer.material.SetTextureScale("_MainTex", new Vector2(-1, 1));
        }
        
    }
}
