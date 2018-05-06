using UnityEngine;
using System.Collections;

public class TreeAppear : MonoBehaviour {

    private TerrainData t;
    private TreeInstance[] originalTrees;
    private TreeInstance[] currentTrees;

    public float depth = 1.0f;
    public float totalTime = 4.0f;
    // Use this for initialization
	IEnumerator Start () {
        t = GetComponent<Terrain>().terrainData;
        originalTrees = t.treeInstances;
        currentTrees = (TreeInstance[])originalTrees.Clone();

        HideTrees();
        yield return null;
	}

    public void HideTrees()
    {
        for (int i = 0; i < t.treeInstanceCount; i++)
        {
            TreeInstance tt = currentTrees[i];
            tt.position = new Vector3(0, -10000.0f, 0);
            currentTrees[i] = tt;
        }
        t.treeInstances = currentTrees;
    }

    public void AnimateTrees()
    {
        currentTrees = (TreeInstance[])originalTrees.Clone();
        for (int i = 0; i < t.treeInstanceCount; i++)
        {
            TreeInstance tt = currentTrees[i];
          
             tt.position = tt.position + new Vector3(0, depth, 0);
            currentTrees[i] = tt;
        }
        t.treeInstances = currentTrees;
        StartCoroutine(anim());
    }
	
	// Update is called once per frame
    private IEnumerator anim () {
        float start = Time.time;
        TreeInstance[] p = (TreeInstance[])currentTrees.Clone();
        while (true)
        {
            for (int i = 0; i < t.treeInstanceCount; i++)
            {
                TreeInstance tt = p[i];
                TreeInstance orig = originalTrees[i];
                tt.position = Vector3.Lerp(tt.position, orig.position, (Time.time - start) / totalTime);
                currentTrees[i] = tt;
            }
            t.treeInstances = currentTrees;
            yield return null;
        }

	}

    void OnApplicationQuit()
    {
        // restore original trees
        t.treeInstances = originalTrees;
    }


}
