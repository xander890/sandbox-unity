using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderUpdates : MonoBehaviour {

    public Slider thresholdSlider;
    public Slider frameSlider;
    public Slider humanBorderSliderL;
    public Slider humanBorderSliderR;
    public Slider humanBorderSliderT;
    public Slider humanBorderSliderD;

    private int L = 20, R = 20, T = 20, D = 20;

    public delegate void OnSliderUpdatedEvent(float value);
    public static event OnSliderUpdatedEvent OnThresholdUpdated;
    public static event OnSliderUpdatedEvent OnFrameNumberUpdated;
    public delegate void OnBorderUpdated(int L, int R, int T, int D);
    public static event OnBorderUpdated OnHumanBorderUpdated;

    void Awake()
    {
        thresholdSlider.onValueChanged.AddListener((value) => OnThresholdUpdated(value));
        frameSlider.onValueChanged.AddListener((value) => OnFrameNumberUpdated(value));
        humanBorderSliderL.onValueChanged.AddListener((value) => { L = (int)value; UpdateBorder(); });
        humanBorderSliderR.onValueChanged.AddListener((value) => { R = (int)value; UpdateBorder(); });
        humanBorderSliderT.onValueChanged.AddListener((value) => { T = (int)value; UpdateBorder(); });
        humanBorderSliderD.onValueChanged.AddListener((value) => { D = (int)value; UpdateBorder(); });
    }

    void UpdateBorder()
    {
        PlayerPrefs.SetInt("L", L);
        PlayerPrefs.SetInt("R", R);
        PlayerPrefs.SetInt("T", T);
        PlayerPrefs.SetInt("D", D);
        OnHumanBorderUpdated(L, R, T, D);
   
    }
	// Use this for initialization
	void Start () {
        L = PlayerPrefs.GetInt("L", 20);
        R = PlayerPrefs.GetInt("R", 20);
        T = PlayerPrefs.GetInt("T", 20);
        D = PlayerPrefs.GetInt("D", 20);
        humanBorderSliderR.value = R;
        humanBorderSliderL.value = L;
        humanBorderSliderD.value = D;
        humanBorderSliderT.value = T;
        Debug.Log("L " +L + " R " + R + " T " + T + "  D " + D);
        OnThresholdUpdated(thresholdSlider.value);
        OnFrameNumberUpdated(frameSlider.value);
        UpdateBorder();
    }
	
}
