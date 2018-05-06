using UnityEngine;
using System.Collections;

public class HideOnButtonPress : MonoBehaviour {

    private KeyCode key = KeyCode.G;
    public GameObject screen;
    public GameObject other_gui;

    private bool isActive = true;

	// Use this for initialization
	void Start () {
        StartCoroutine(UpdateActive());
        Cursor.visible = true;
	}
	
	// Update is called once per frame
	IEnumerator UpdateActive () {
        while (true)
        {
            if(Input.GetKey(KeyCode.Space))
            {
                screen.SetActive(false);
                Tracker.isEnabled = true;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Cursor.visible = false;
                screen.SetActive(true);
                Tracker.isEnabled = false;
                for (int i = 0; i < other_gui.transform.childCount; i++)
                {
                    other_gui.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else if (Input.GetKey(key))
            {
                isActive = !isActive;
                Cursor.visible = isActive;
                for (int i = 0; i < other_gui.transform.childCount; i++)
                {
                    other_gui.transform.GetChild(i).gameObject.SetActive(isActive);
                }
                yield return new WaitForSeconds(0.5f);
            }
            yield return null;
        }
    }
}
