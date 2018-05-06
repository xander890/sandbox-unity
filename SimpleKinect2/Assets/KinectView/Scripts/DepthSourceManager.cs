using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class DepthSourceManager : MonoBehaviour
{   
    private static KinectSensor _Sensor;
    private DepthFrameReader _Reader;
    private ushort[] _Data;
    public float updateTimeMillis = 1.0f;

    public delegate void DepthFrameUpdate(ushort[] data, int width, int height);
    public static event DepthFrameUpdate DepthFrameUpdateEvent;


    public ushort[] GetData()
    {
        return _Data;
    }

    public static int Width() { return _Sensor.DepthFrameSource.FrameDescription.Width; }
    public static int Height() { return _Sensor.DepthFrameSource.FrameDescription.Height; }

    void Start () 
    {
        _Sensor = KinectSensor.GetDefault();
        
        if (_Sensor != null) 
        {
            _Reader = _Sensor.DepthFrameSource.OpenReader();
            _Data = new ushort[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels];
           
        }
        StartCoroutine(UpdateDepth());
    }
    
    IEnumerator UpdateDepth () 
    {
        while (true)
        {
            if (_Reader != null)
            {
                var frame = _Reader.AcquireLatestFrame();
                if (frame != null)
                {
                    frame.CopyFrameDataToArray(_Data);
                    frame.Dispose();
                    DepthFrameUpdateEvent(_Data, Width(), Height());
                    frame = null;
                }
            }
            yield return new WaitForSeconds(updateTimeMillis / 1000.0f);
        }
    }
    
    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }
        
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }
}
