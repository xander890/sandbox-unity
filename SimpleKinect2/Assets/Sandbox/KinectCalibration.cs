using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;
using System;
using System.Xml.Linq;
using System.Drawing;


public class KinectCalibration : MonoBehaviour {

    public Vector2Int projectorResolution;
    public Vector2Int kinectResolution;
    public Matrix4x4 projectionMatrix;


    Vector2Int parseVector2Int(string s)
    {
        string[] ss = s.Split(',');
        Vector2Int res = new Vector2Int();
        res.Set(Int32.Parse(ss[0]), Int32.Parse(ss[1]));
        return res;
    }


    void loadCalibration()
    {
        string xml_data = Application.dataPath + "/StreamingAssets/settings/calibration.xml";
        XDocument doc = XDocument.Load(xml_data);
        string projectorResolutionS = (string)doc.Element("CALIBRATION").Element("RESOLUTIONS").Element("PROJECTOR");
        projectorResolution = parseVector2Int(projectorResolutionS);
        string kinectResolutionS = (string)doc.Element("CALIBRATION").Element("RESOLUTIONS").Element("KINECT");
        kinectResolution = parseVector2Int(kinectResolutionS);
        var ecoefficients = doc.Element("CALIBRATION").Element("COEFFICIENTS");
        projectionMatrix = Matrix4x4.identity;
        for (int i = 0; i < 11; i++)
        {
            string coeff_string = "COEFF" + i;
            float value = System.Single.Parse((string)ecoefficients.Element(coeff_string));
            int row = i / 4;
            int column = i % 4;
            projectionMatrix[row, column] = value;
        }
        projectionMatrix[2, 3] = 1.0f;
    }

	// Use this for initialization
	void Start () {
        loadCalibration();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
