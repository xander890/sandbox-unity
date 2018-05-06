using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorViewInitialize : MonoBehaviour {



	// Use this for initialization
	void Start () {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        rectTransform.localScale = new Vector3(1, 1, 1);
        //rectTransform
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
