using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class OnCalibrate : MonoBehaviour {

    public delegate void OnCalibrateButtonClicked();
    public static event OnCalibrateButtonClicked OnCalibrateEvent;

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(() => OnCalibrateEvent());
	}
	
}
