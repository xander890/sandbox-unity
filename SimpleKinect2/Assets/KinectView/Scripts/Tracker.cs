using UnityEngine;
using System.Collections;
using Emgu.CV.Structure;
using System.Collections.Generic;


public class Tracker : MonoBehaviour {

    public GameObject spawnPrefab;
    private GameObject tracked;
    public bool debug = false;

    public static bool isEnabled = false;

    public delegate void OnStableRectangleUpdateDelegate(Transform t, RotatedRect r);
    public static event OnStableRectangleUpdateDelegate OnStableRectangleEvent;

    public delegate void OnDisappearDelegate();
    public static event OnDisappearDelegate OnDisappearEvent;

    public static event OnStableRectangleUpdateDelegate OnUnstableRectangleEvent;


    private float position_history = 0.0f;
    private RotatedRect last_seen_rect = RotatedRect.Empty;

    public int frames_to_stabilize = 10;
    private int current_frame = 0;
    private bool detected = false;

    [Range(0, 10)]
    public float detectionDelay = 0.5f;

	// Use this for initialization
	void Start () {
        RectangleDetectorUnity.UpdateRectangleEvent += OnRectangleUpdate;
        tracked = (GameObject) GameObject.Instantiate(spawnPrefab, Vector3.zero, Quaternion.identity);

	}
	
	// Update is called once per frame
	void Update () {
        if (debug)
        {
            RotatedRect test = new RotatedRect(new System.Drawing.PointF(100.0f, 300.0f), new System.Drawing.SizeF(70, 10), 20.0f);
            List<RotatedRect> l = new List<RotatedRect>();
            l.Add(test);
            //Debug.Log(Time.time * 0.1f);
            OnRectangleUpdate(l);

        }
	}

    void OnRectangleUpdate(List<RotatedRect> l)
    {
        if (!isEnabled)
            return;
        if ( l.Count > 0)
        {
            if (OnUnstableRectangleEvent != null)
            {
                int width = DepthSourceManager.Width();
                int height = DepthSourceManager.Height();

                if (debug)
                    width = height = 512;

                RotatedRect copy = l[0];
                float r_w = copy.Size.Width;
                float r_h = copy.Size.Height;
                float angle = copy.Angle;

                if (r_h > r_w)
                {
                    r_h = copy.Size.Width;
                    r_w = copy.Size.Height;
                    angle = angle - 90.0f;
                }

                Vector2 centre = new Vector2(l[0].Center.X / width, l[0].Center.Y / height);
                centre = centre * 2 - new Vector2(1, 1);
                centre = -centre;
                tracked.transform.position = new Vector3(centre.x, 0, centre.y) + transform.position;
                tracked.transform.localScale = new Vector3(r_w / width, 0.2f, r_h / height) * 2;
                tracked.transform.rotation = Quaternion.Euler(0, -angle, 0);

                OnUnstableRectangleEvent(tracked.transform, copy);
            }

            if (!detected)
            {
                if (last_seen_rect.Equals(RotatedRect.Empty))
                {
                    last_seen_rect = l[0];
                    return;
                }

                current_frame++;
                float delta_pos = (float)Vector2.Distance(new Vector2(l[0].Center.X, l[0].Center.Y), new Vector2(last_seen_rect.Center.X, last_seen_rect.Center.Y));
                float new_pos_h = position_history * 0.75f + delta_pos * 0.25f;
             //   Debug.Log("History: " + Mathf.Abs(new_pos_h - position_history));
                float diff = Mathf.Abs(new_pos_h - position_history);
                position_history = new_pos_h;

                if (current_frame < frames_to_stabilize || diff > 0.1f)
                {
                    return;
                }

                last_seen_rect = l[0];

                int width = DepthSourceManager.Width();
                int height = DepthSourceManager.Height();

                if (debug)
                    width = height = 512;

                RotatedRect copy = l[0];
                float r_w = copy.Size.Width;
                float r_h = copy.Size.Height;
                float angle = copy.Angle;

                if (r_h > r_w)
                {
                    r_h = copy.Size.Width;
                    r_w = copy.Size.Height;
                    angle = angle - 90.0f;
                }

                Vector2 centre = new Vector2(l[0].Center.X / width, l[0].Center.Y / height);
                centre = centre * 2 - new Vector2(1, 1);
                centre = -centre;
                tracked.transform.position = new Vector3(centre.x, 0, centre.y) + transform.position;
                tracked.transform.localScale = new Vector3(r_w / width, 0.2f, r_h / height) * 2;
                tracked.transform.rotation = Quaternion.Euler(0, -angle, 0);



                if (OnStableRectangleEvent != null)
                {
                    StartCoroutine(stableRectEvent(tracked.transform, copy));
                }
                   


                detected = true;
            }
        }
        else
        {
            //current_frame = Mathf.Max(0, current_frame - 1);

            //if(current_frame == 0)
            //{
            //    detected = false;
            //    if (OnDisappearEvent != null)
            //        OnDisappearEvent();
            //    tracked.transform.position = Vector3.one * -100000.0f;
            //}
        }

        
    }


    IEnumerator stableRectEvent(Transform t, RotatedRect r)
    {
        yield return new WaitForSeconds(detectionDelay);
        OnStableRectangleEvent(t, r);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0, 0.5f, 0), new Vector3(2, 1, 2));
    }
}
