using UnityEngine;
using System.Collections;

public class ParticleEffects : MonoBehaviour {

    public GameObject[] startingParticlesPrefabs;
    public GameObject[] fireworksPrefabs;
    public SetPivot pivot;
    public float sphereRadius = 1.0f;
    public float totalTime = 10.0f;

	// Use this for initialization
	void Start () {
        SetPivot.OnAnimationCompleted += StartFireworksAnimation;
     //   SetPivot.OnAnimationStarted += StartInitialAnimation;
        
	}

    public void StartFireworksAnimation()
    {
        StartCoroutine(fireworksPlay());
    }

    public void StartInitialAnimation(Vector3 pos)
    {
        StartCoroutine(startingPlay(pos));
    }

    IEnumerator fireworksPlay()
    {
  
        float t = 0.0f;
        AudioSource audio = this.GetComponent<AudioSource>();
        audio.Play();

        while(t < totalTime)
        {
            Vector3 point = Random.onUnitSphere * sphereRadius;
            point = new Vector3(point.x, Mathf.Abs(point.y), point.z);
            GameObject pivot = GameObject.Find("pivot");
            GameObject prefab = fireworksPrefabs[(int)(Random.value * fireworksPrefabs.Length)];
            GameObject.Instantiate(prefab, transform.position + point + pivot.transform.position, Quaternion.identity);
            t += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);    
        }
        audio.Stop();
    }


    IEnumerator startingPlay(Vector3 pos)
    {
        if (startingParticlesPrefabs.Length < 1)
            yield break;
        float startRings = Time.time;
       // GameObject pivot = GameObject.Find("pivot");
        Vector3 offset = new Vector3(0.0f, 0.05f, 0.0f);
        GameObject prefab = startingParticlesPrefabs[(int)(Random.value * startingParticlesPrefabs.Length)];
        GameObject particle = (GameObject)GameObject.Instantiate(prefab, pos + offset, prefab.transform.rotation);
        Renderer[] materials = particle.GetComponentsInChildren<Renderer>();
        foreach (Renderer m in materials)
        {
            m.material.SetFloat("_Scale", 0.3f);
            m.material.SetVector("_Translation", particle.transform.position+new Vector3(0.0f,0.2f,0.0f));
        }
        particle.transform.localScale = particle.transform.localScale * 0.1f;
        while (true)
        {
            float t = 1 - (Time.time - startRings) / (pivot.appearTime + pivot.movingTime);
            Debug.Log(t);
            if (t < 0)
            {
                Destroy(particle);
                yield break;
            }
            yield return new WaitForSeconds(0.01f);     
        } 
        
    }



    // Update is called once per frame

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, sphereRadius);
    }
}
