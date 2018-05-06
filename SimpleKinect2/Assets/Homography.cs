using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;


public class Homography : MonoBehaviour {

    private bool isListening = false;
    private const float cm_to_units = 1.0f / 100.0f; // 1 meter = 2 units
    private const float calibration_artefact = 22.0f * cm_to_units;

	// Use this for initialization
	void Start () {
        Tracker.OnStableRectangleEvent += updatedRect;
        OnCalibrateHomography.OnCalibrateHomographyEvent += calibrateHomography;

        var test = new PointF[4];
        test[0] = new PointF(210.3f, 198.2f);
        test[1] = new PointF(207.0f, 249.2f);
        test[2] = new PointF(275.84f, 
            253.5f);
        test[3] = new PointF(279.03f,202.5f);

        calibrateHomographyMaths(test, 512, 424);
	}

    private PointF get_closest(PointF[] points, PointF to_compare)
    {
        return points.Select(p => new { Point = p, Distance2 = (p.X - to_compare.X) * (p.X - to_compare.X) + (p.Y - to_compare.Y) * (p.Y - to_compare.Y) }).Aggregate((p1, p2) => p1.Distance2 < p2.Distance2 ? p1 : p2).Point;
    }

    
    private PointF get_camera_point(Vector3 point)
    {
        Vector3 p = Camera.main.WorldToScreenPoint(point);
        return new PointF(p.x, p.y);
    }

    private void updatedRect(Transform t, RotatedRect r)
    {
        if (!isListening)
            return;

        var points_kinect = r.GetVertices();
        calibrateHomographyMaths(points_kinect, DepthSourceManager.Width(), DepthSourceManager.Height());
    }


    private void calibrateHomographyMaths(PointF[] points_kinect, int w, int h)
    {
        Debug.Log("Calbirating homography...");
        var points_sorted = new PointF[4];
        points_sorted[0] = get_closest(points_kinect, new PointF(0, 0));
        points_sorted[1] = get_closest(points_kinect, new PointF(0, h));
        points_sorted[2] = get_closest(points_kinect, new PointF(w, h));
        points_sorted[3] = get_closest(points_kinect, new PointF(w, 0));
        Debug.Log("WWWWW: " + w + " HHHHHH: " + h);
        var points_camera = new PointF[4];
        points_camera[0] = get_camera_point(new Vector3(0.5f, 0, 0.5f) * calibration_artefact);
        points_camera[1] = get_camera_point(new Vector3(0.5f, 0, -0.5f) * calibration_artefact);
        points_camera[2] = get_camera_point(new Vector3(-0.5f, 0, -0.5f) * calibration_artefact);
        points_camera[3] = get_camera_point(new Vector3(-0.5f, 0, 0.5f) * calibration_artefact);

        for (int i = 0; i < 4; i++)
        {
            points_sorted[i] = new PointF(points_sorted[i].X / w, points_sorted[i].Y / h);
        }
        Debug.Log(points_sorted[0] + " " + points_camera[0]);
        Debug.Log(points_sorted[1] + " " + points_camera[1]);
        Debug.Log(points_sorted[2] + " " + points_camera[2]);
        Debug.Log(points_sorted[3] + " " + points_camera[3]);

        Emgu.CV.Matrix<double> homography = new Emgu.CV.Matrix<double>(3, 3);
        CvInvoke.FindHomography(points_camera, points_sorted, homography, Emgu.CV.CvEnum.HomographyMethod.LMEDS);
        
        Debug.Log(homography[0, 0] + " " + homography[0, 1] + " " + homography[0, 2]);
        Debug.Log(homography[1, 0] + " " + homography[1, 1] + " " + homography[1, 2]);
        Debug.Log(homography[2, 0] + " " + homography[2, 1] + " " + homography[2, 2]);

        UnityEngine.Matrix4x4 m = Matrix4x4.identity;
        m.m00 = (float)homography[0, 0];
        m.m01 = (float)homography[0, 1];
        m.m02 = (float)homography[0, 2];
        m.m10 = (float)homography[1, 0];
        m.m11 = (float)homography[1, 1];
        m.m12 = (float)homography[1, 2];
        m.m20 = (float)homography[2, 0];
        m.m21 = (float)homography[2, 1];
        m.m22 = (float)homography[2, 2];

        this.GetComponent<MeshRenderer>().material.SetMatrix("_Homography", m);
//        this.GetComponent<MeshRenderer>().material.SetVector("_KinectDims", new Vector2(w, h));
    }

    private void calibrateHomography()
    {
        isListening = true;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
