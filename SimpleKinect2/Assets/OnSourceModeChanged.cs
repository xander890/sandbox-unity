using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OnSourceModeChanged : MonoBehaviour
{
    public delegate void OnSourceChanged(int value);
    public static event OnSourceChanged OnSourceChangedEvent;

    // Use this for initialization
    void Start()
    {
        GetComponent<Dropdown>().onValueChanged.AddListener((value) => OnSourceChangedEvent(value));
    }

}
