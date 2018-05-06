using UnityEngine;
using System.Collections;
using UnityEngine;
using System.Collections;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using System;
using System.Drawing;

public class TestCV : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    Image<Bgr, byte> picture = new Image<Bgr, byte>("C:\\Users\\alcor\\Desktop\\picture1.jpg"); 
        Bgr myWhiteColor = new Bgr(255, 255, 255);
        for (int i=0; i<200; i++)
        { picture[i, i] = myWhiteColor; }
        picture.Save("C:\\Users\\alcor\\Desktop\\picture2.jpg");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
