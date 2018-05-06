using UnityEngine;
using System.Collections;
using Emgu.CV.Structure;
using System.Collections.Generic;

public class RectangleGlow : MonoBehaviour {

    public bool isEnabled = true;
    private MeshRenderer renderer;
	// Use this for initialization
	void Start () {
        renderer = GetComponent<MeshRenderer>();
        RectangleDetectorUnity.UpdateRectangleEvent += OnRectangleUpdate;
        DepthSourceManager.DepthFrameUpdateEvent += DepthUpdated;
    }

    void OnRectangleUpdate(List<RotatedRect> l)
    {
        if (!isEnabled)
            return;

        if (l.Count == 0)
        {
            renderer.material.SetInt("_enable_rect", 0);
            return;
        }
        renderer.material.SetInt("_enable_rect", 1);
        int width = DepthSourceManager.Width();
        int height = DepthSourceManager.Height();
        System.Drawing.PointF[] points = l[0].GetVertices();
        Vector4[] positions = new Vector4[4];
        for (int j = 0; j < 4; j++)
        {
            Vector2 point = new Vector2(points[j].X / width, points[j].Y / height);
            point = point * 2 - new Vector2(1, 1);
            //point = -point;
            positions[j] = new Vector4(point.x, point.y, 0, 0);
        }
        renderer.material.SetVectorArray("rectangle_points", positions);
    }



    private Texture2D _Texture;
    private Color[] converted_data;        

    private void DepthUpdated(ushort[] depth, int width, int height)
    {
        

        if(_Texture == null)
        {
            converted_data = new Color[width * height];
            for (int i = 0; i < converted_data.Length; i++)
                converted_data[i] = new Color(0, 0, 0, 0);
            _Texture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            renderer.material.SetTexture("_Depth", _Texture);
        }


        for (int i = 0; i < depth.Length; i++)
            converted_data[i] = new Color(0.0f, ((float)depth[i]) / 65535.0f, 0.0f, 0.0f);


        _Texture.SetPixels(converted_data);
        _Texture.Apply();
    }

}
