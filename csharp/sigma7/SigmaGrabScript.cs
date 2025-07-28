using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Warning! You probably shouldn't use this script in your own project
//(since it's pretty innefecient and thrown together hacky-like).
//Best to use it at as refernce on how to utilize the library.


public class SigmaGrabScript : MonoBehaviour
{
    private GameObject Thumb;
    private GameObject Index;

    private GameObject MidPoint;
    private Rigidbody rb;
    private GameObject RedCube;
    private GameObject SuperParent;
    private Color OriginalColor;
    public bool ChangeColorWhenGrabbed;
    public bool UseGravity;

    private GameObject TopCube;
    private GameObject MidCube;
    private GameObject BaseCube;

    private GameObject GrabbingCube;

    private List<GameObject> CollisionList = new List<GameObject>();

    public float forceX;
    public float forceY;
    public float forceZ;

    // Start is called before the first frame update
    void Start()
    {
        ChangeColorWhenGrabbed = false;
        RedCube = gameObject.transform.parent.gameObject;
        rb = RedCube.GetComponent<Rigidbody>();

        //Thumb = GameObject.FindWithTag("Thumb");
        //Index = GameObject.FindWithTag("Index");
        Thumb = GameObject.FindWithTag("ThumbProxy");
        Index = GameObject.FindWithTag("IndexProxy");
        MidPoint = GameObject.FindWithTag("MidPoint");
        GrabbingCube = GameObject.FindWithTag("GrabbingCube");

        MidCube = GameObject.FindWithTag("MidCube");
        BaseCube = GameObject.FindWithTag("BaseCube");
        TopCube = GameObject.FindWithTag("TopCube");

        SuperParent = gameObject.transform.parent.gameObject;
        OriginalColor = RedCube.GetComponent<Renderer>().material.color;

        
    }

    //OnTriggerEnter and OnTriggerExit together keep a list of all objects currently in collision with this
    void OnTriggerEnter(Collider Other)
    {
        CollisionList.Add(Other.gameObject);
    }

    void OnTriggerExit(Collider Other)
    {
        CollisionList.Remove(Other.gameObject);
    }


    void FixedUpdate()
    {


        double[] tempArray = new double[1];

        HapticNativePlugin.get_gripper_force(tempArray);

        //Debug.Log(tempArray[0]);

        double[] array = new double[3];

        HapticNativePlugin.get_gripper_angle_deg(array);

        //Debug.Log(array[2]);

        //Checks to initiate grabbing
        //First check that both the thumb and index are in contact with this
        if (CollisionList.Contains(Thumb) && CollisionList.Contains(Index))
        {
            //Check that the grasper is at least somewhat open (to prevent grabbing with a closed grasper)
            //and check that the grasper is not currently holding any cube
            if (array[0] > 3 && TopCube.transform.parent != MidPoint.transform && BaseCube.transform.parent != MidPoint.transform && MidCube.transform.parent != MidPoint.transform)
            {

                HapticNativePlugin.enable_grasping_mode(true, 0, 0, 0.1);
                ///////////////////////////////////////////////////////////////
                //                          NOTE!!!                          //
                //Sleeping for 1 millisecond here instead of actually dealing//
                //with a race condition is deeply irresponsible!             //
                //Don't tell your parents!                                   //
                ///////////////////////////////////////////////////////////////
                
                System.Threading.Thread.Sleep(1);
                GrabbingCube.GetComponent<GrabbingCube>().SetCube(gameObject);
                SuperParent.transform.parent = MidPoint.transform;
                rb.useGravity = false;

                if (ChangeColorWhenGrabbed)
                {
                    RedCube.GetComponent<Renderer>().material.color = Color.white;
                }

                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
                
                //Functionality not currently implemented
                if (UseGravity)
                {
                    //Debug.Log(HapticNativePlugin.GetToolForce());
                    //Vector3 forceVector = new Vector3(forceX, forceY, forceZ);
                    //HapticNativePlugin.enable_static_tool_force();
                    //HapticNativePlugin.set_static_tool_force(forceVector);

                }

            }
        }
        else
        {
            //Check if cube is registered as being grabbed even though we are no longer grabbing it
            if (GrabbingCube.GetComponent<GrabbingCube>().CurrentCube() == gameObject)
            {

                GrabbingCube.GetComponent<GrabbingCube>().DisableCube();

                HapticNativePlugin.disable_grasping_mode();

                HapticNativePlugin.disable_static_tool_force();
                rb.isKinematic = false;
                SuperParent.transform.parent = null;
                rb.useGravity = true;

                    RedCube.GetComponent<Renderer>().material.color = OriginalColor;
                
                rb.constraints = RigidbodyConstraints.None;
            }

        }
    }
}
