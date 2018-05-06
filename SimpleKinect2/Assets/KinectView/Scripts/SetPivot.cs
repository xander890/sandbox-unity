using UnityEngine;
using System.Collections;

using System.Collections;
using Emgu.CV.Structure;
public class SetPivot : MonoBehaviour {

    private MeshRenderer[] renderers;
    public ReflectionProbe cubemap;
    public delegate void AnimationDelegate();

    public static event AnimationDelegate OnAnimationCompleted;
    public static event AnimationDelegate OnAnimationStarted;
    public static event AnimationDelegate OnHouseCollapseStarted;
    public AnimationCurve TimeCurve;



    [Range(0, 1)]
    public float maxDistance = 1.0f;

    [Range(0, 10)]
    public float movingTime = 1.0f;

    [Range(0, 10)]
    public float appearTime = 2.0f;

    [Range(0, 1)]
    public float wigglyness = 0.005f;

    public float minimal = 5.0f;
    public float maximal = 10.0f;

    public const float bakedScale = 23.0f;

    public bool enableAnimation = true;
    private bool started = false;
    Vector3 origo_scale;
    // Use this for initialization
	void Start () {
        renderers = GetComponentsInChildren<MeshRenderer>();
        Tracker.OnStableRectangleEvent += updatedRect;
        OnCalibrate.OnCalibrateEvent += CalibrateRequest;
        Tracker.OnDisappearEvent += rectangleAway;
	}

	private void CalibrateRequest()
    {
        started = false;
    }

    private void rectangleAway()
    {
        if(!started)
        {
            transform.position = Vector3.one * -100000.0f;
        }
    }

    private void updatedRect(Transform t, RotatedRect r)
    {
        if (!enableAnimation || !started)
        {
            started = true;
            transform.position = new Vector3(t.position.x, 0, t.position.z);
            transform.rotation = t.rotation;
            float maxScale = Mathf.Max(t.localScale.x / 2.0f, t.localScale.z);
            float clampedScale = Mathf.Clamp(maxScale, minimal, maximal);
            transform.localScale = Vector3.one * clampedScale * bakedScale;
            if (enableAnimation)
            {
                StartHouseAnimation();
            }
        }
    }

    private void cruisingRect(Transform t, RotatedRect r)
    {
        transform.position = new Vector3(t.position.x, 0, t.position.z);
        transform.rotation = t.rotation;
        float maxScale = Mathf.Max(t.localScale.x / 2.0f, t.localScale.z);
        float clampedScale = Mathf.Clamp(maxScale, minimal, maximal);
        transform.localScale = Vector3.one * clampedScale * bakedScale;
    }

    public void StartHouseAnimation()
    {
        origo_scale = transform.localScale;
        StartCoroutine(StartAnimation());
    }


    private IEnumerator StartAnimation()
    {

        yield return AnimateHouse();
            
    }



    private IEnumerator AnimateHouse()
    {
        if (OnHouseCollapseStarted != null)
            OnHouseCollapseStarted();
        float startAnimateHouse = Time.time;
        while (true)
        {
            float t = Mathf.Clamp01((Time.time - startAnimateHouse) / movingTime);
            transform.localScale = origo_scale * TimeCurve.Evaluate(t);

            if (t > 0.999f)
            {
                started = false;
                if (OnAnimationCompleted != null)
                    OnAnimationCompleted();
                Tracker.OnUnstableRectangleEvent += cruisingRect;
                Tracker.OnStableRectangleEvent -= updatedRect;
                yield break; // Cleanup when you are done
            }
            yield return null;
        }       
	}

   
}
