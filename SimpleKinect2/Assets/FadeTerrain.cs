using UnityEngine;
using System.Collections;

public class FadeTerrain : MonoBehaviour {

    public float fadeTime;
    private Material terra_mat;

    void Start()
    {
        terra_mat = GetComponent<Terrain>().materialTemplate;
        terra_mat.SetFloat("_alphaColor", 0.0f);
        SetPivot.OnHouseCollapseStarted += startFadeAnimation;
    }

    private void startFadeAnimation()
    {
        StartCoroutine(startFade());
    }


	public IEnumerator startFade()
    {
        float start = Time.time;
        float t = 0.0f;
        GetComponent<TreeAppear>().AnimateTrees();
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            terra_mat.SetFloat("_alphaColor", t / fadeTime);
            yield return null;
        }
       
    }
}
