using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;
using System;
using System.Xml.Linq;
using System.Drawing;

public class KinectSettings : MonoBehaviour
{
    public Rectangle kinectROI;
    public Vector3 basePlaneNormal;
    public Vector3 basePlaneOffset;
    public float maxOffsetBack;
    public int averagingSlots;
    public bool followBigChanges;
    public bool outlierInpainting;
    public bool fullFrameFiltering;
    public bool spatialFiltering;


    Vector3 parseVector3(string s)
    {
        string[] ss = s.Split(',');
        Vector3 res = new Vector3();
        res.Set(float.Parse(ss[0]), float.Parse(ss[1]), float.Parse(ss[2]));
        return res;
    }

    void loadKinectProjector()
    {
        string xml_data = Application.dataPath + "/StreamingAssets/settings/kinectProjectorSettings.xml";
        XDocument doc = XDocument.Load(xml_data);
        var root = doc.Element("KINECTSETTINGS");
        kinectROI = new Rectangle();
        string[] roi = ((string)root.Element("kinectROI")).Split(',');
        kinectROI.X = Int32.Parse(roi[0]);
        kinectROI.Y = Int32.Parse(roi[1]);
        kinectROI.Width = Int32.Parse(roi[3]);
        kinectROI.Height = Int32.Parse(roi[4]);
        basePlaneNormal = parseVector3((string)root.Element("basePlaneNormalBack"));
        basePlaneOffset = parseVector3((string)root.Element("basePlaneOffsetBack"));
        maxOffsetBack = float.Parse((string)root.Element("maxOffsetBack"));
        averagingSlots = Int32.Parse((string)root.Element("numAveragingSlots"));
        followBigChanges = ((string)root.Element("followBigChanges")) == "0" ? false : true;
        outlierInpainting = ((string)root.Element("OutlierInpainting")) == "0" ? false : true;
        fullFrameFiltering = ((string)root.Element("FullFrameFiltering")) == "0" ? false : true;
        spatialFiltering = ((string)root.Element("spatialFiltering")) == "0" ? false : true;
    }


    // Use this for initialization
    void Start()
    {
        loadKinectProjector();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
