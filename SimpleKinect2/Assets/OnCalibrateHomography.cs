using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class OnCalibrateHomography : MonoBehaviour
{

    public delegate void OnCalibrateHomographyButtonClicked();
    public static event OnCalibrateHomographyButtonClicked OnCalibrateHomographyEvent;

    // Use this for initialization
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => OnCalibrateHomographyEvent());
    }

}
