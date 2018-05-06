using UnityEngine;
using System.Collections;

public class FadePlane : MonoBehaviour {

    public float speed = 0.1f;

	// Use this for initialization
	void Start () {
        SetPivot.OnHouseCollapseStarted += startMove;
	}
	
    private void startMove()
    {
        StartCoroutine(move());
    }

	// Update is called once per frame
	IEnumerator move () {
	    while(true)
        {
            transform.Translate(new Vector3(0, -Time.deltaTime * speed, 0));
            yield return null;
        }
	}
}
