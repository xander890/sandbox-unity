using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;

public class DepthSourceColorView : MonoBehaviour
{
    public float alpha = 0.7f;
    public float depth_multiplier = 10.0f;

    private Texture2D _Texture = null;
    private Color[] converted_data;        
            
    void Start ()
    {
        DepthSourceManager.DepthFrameUpdateEvent += DepthUpdated;
    }

    private void DepthUpdated(ushort[] depth, int width, int height)
    {
        

        if(_Texture == null)
        {
            converted_data = new Color[width * height];
            for (int i = 0; i < converted_data.Length; i++)
                converted_data[i] = new Color(0, 0, 0, 0);
            _Texture = new Texture2D(width, height, TextureFormat.RFloat, false);
            gameObject.GetComponent<RawImage>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));

            gameObject.GetComponent<RawImage>().texture = _Texture;
        }


        for (int i = 0; i < depth.Length; i++)
            converted_data[i] = alpha * converted_data[i] + (1 - alpha) * new Color(((float)depth[i]) / 65535.0f * depth_multiplier, 0.0f, 0.0f, 0.0f);


        _Texture.SetPixels(converted_data);
        _Texture.Apply();
    }
}
