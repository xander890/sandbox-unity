using UnityEngine;
using System.Collections;
using ElementSimulator;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using UnityEngine.UI;
public class RectangleDebug : MonoBehaviour {

    private Texture2D _Texture = null;
    private Color[] converted_data;

	// Use this for initialization
	void Start () {
        RectangleDetectorUnity.DebugRectangleEvent += UpdatedRect;
	}
	
	// Update is called once per frame
	void UpdatedRect (byte[] bg, byte[] rects) {

        if(!gameObject.activeInHierarchy)
        {
            return;
        }

        int width = DepthSourceManager.Width();
        int height = DepthSourceManager.Height();
        if (_Texture == null)
        {
            converted_data = new Color[width * height];
            for (int i = 0; i < converted_data.Length; i++)
                converted_data[i] = new Color(0, 0, 0, 0);
            _Texture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            gameObject.GetComponent<RawImage>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));
            gameObject.GetComponent<RawImage>().texture = _Texture;
        }

        for (int i = 0; i < bg.Length; i++)
            converted_data[i] = new Color((float)bg[i] / 255.0f * 10, (float)rects[i] / 255.0f * 10, 0.0f, 0.0f);

        _Texture.SetPixels(converted_data);
        _Texture.Apply();
	}
}
