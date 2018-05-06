using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class wreckingBall : MonoBehaviour
{
    public Windows.Kinect.JointType _jointType;
    public GameObject _bodySourceManager;
    private BodySourceManager _bodyManager;
    private Rigidbody rb;
    public float speed;
    // A difference vector
    private Vector3 diffVec = new Vector3(0f, 0f, 0f);
    // Contains the position from the previous iteration
    private Vector3 x1 = new Vector3(0f, 0f, 0f);
    // Contains the position from the current iteration
    private Vector3 x2 = new Vector3(0f, 0f, 0f);
    // private Vector3 initPos = new Vector3(-4.5f,-0.2f,2.96f);
    // Holds the hand position so that we can map the position to our play ground.
    private Vector3 handPos = new Vector3(0f,0f,0f);
    // private Vector3 tempPos = new Vector3(0f,0f,0f);
    private int checker = 0;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_bodySourceManager == null)
        {
            return;
        }

        _bodyManager = _bodySourceManager.GetComponent<BodySourceManager>();
        if (_bodyManager == null)
        {
            return;
        }

        Body[] data = _bodyManager.GetData();
        if (data == null)
        {
            return;
        }

        // get the first tracked body...
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                // A major hack, this should ensure that we are already tracking the hand
                if(Mathf.Abs(handPos.x)<0.1 & checker < 100 )
                {
                    var posHand = body.Joints[_jointType].Position;
                    handPos.x = posHand.X;
                    handPos.y = posHand.Y;
                    handPos.z = posHand.Z;
                    checker++;
                }
                // Get the joint position for hour joint type (in our case, the right hand)
                var pos = body.Joints[_jointType].Position;
                // Set x2 to our current position
                x2.x = pos.X;
                x2.y = pos.Y;
                x2.z = pos.Z;
                // Sets up a difference vector to figure out how to move the wrecking ball
                diffVec = x2 - x1;
                // Because of the view we reverse the z-direction
                diffVec.z = -diffVec.z;
                // Update x1 (old value) with the current.
                x1 = x2;
                // Only apply the force if the absolute sum is larger than 0.2 
                if (Mathf.Abs(diffVec.x)+ Mathf.Abs(diffVec.y) + Mathf.Abs(diffVec.z) >0.2)
                {
                    // Add the force to the wrecking balls rigidbody
                    rb.AddForce(diffVec * speed);
                }
                break;
            }
        }
    }
}