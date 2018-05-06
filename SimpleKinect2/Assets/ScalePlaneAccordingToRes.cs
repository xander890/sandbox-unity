using UnityEngine;
using System.Collections;

public class ScalePlaneAccordingToRes : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("Depth camera: " + DepthSourceManager.Width() + " " + DepthSourceManager.Height());
        float lsz = DepthSourceManager.Height() / ((float)DepthSourceManager.Width());
        float lsx = lsz * ColorSourceManager.ColorWidth / ((float)ColorSourceManager.ColorHeight);
        transform.localScale = 0.2f * new Vector3(lsx, 5.0f, lsz);
    }
}
