using UnityEngine;
using System.Collections;
using ElementSimulator;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;

public class RectangleDetectorUnity : MonoBehaviour {

    public delegate void UpdateRectangleFunc(List<RotatedRect> rectangles);
    public delegate void DebugRectangleFunc(byte[] image, byte[] rectimage);

    public static event UpdateRectangleFunc UpdateRectangleEvent;
    public static event DebugRectangleFunc DebugRectangleEvent;

    ElementSimulator.RectangleDetector rect = null;


    private int threshold = 60;
    private int memory = 30;
    private int borderL, borderR, borderT, borderD = 20;
	// Use this for initialization
	void Awake () {
        DepthSourceManager.DepthFrameUpdateEvent += DepthUpdated;
        OnCalibrate.OnCalibrateEvent += CalibrateRequest;
        SliderUpdates.OnFrameNumberUpdated += updateFrames;
        SliderUpdates.OnThresholdUpdated += updateThreshold;
        SliderUpdates.OnHumanBorderUpdated += updateBorder;
	}

    private void updateThreshold(float value) { threshold = (int)value;}
    private void updateFrames(float value) { memory = (int)value;}
    private void updateBorder(int L, int R, int T, int D) { borderL = L; borderR = R; borderT = T; borderD = D; }

	private void CalibrateRequest()
    {
        if(rect != null)
        {
            rect.Calibrate();
        }
    }

    private void DepthUpdated(ushort[] depth, int width, int height)
    {
        if (rect == null)
        {
            rect = new RectangleDetector();
            rect.SetResolution(width, height);
        }
        rect.OffsetMM_int = threshold;
        rect.FrameMemory_int = memory;
        rect.HumanBorderL = borderL;
        rect.HumanBorderR = borderR;
        rect.HumanBorderT = borderT;
        rect.HumanBorderD = borderD;
        List<RotatedRect> res = rect.DepthReady(depth);

        if (DebugRectangleEvent != null)
            DebugRectangleEvent(rect.GetThreshold(), rect.GetRects());
        if(UpdateRectangleEvent != null)
            UpdateRectangleEvent(res);
    }
}
