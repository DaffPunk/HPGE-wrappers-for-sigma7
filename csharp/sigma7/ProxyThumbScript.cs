using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxyThumbScript : MonoBehaviour
{
    private GameObject MidPoint;
    private GameObject Thumb;

    // Start is called before the first frame update
    void Start()
    {
        MidPoint = GameObject.FindWithTag("MidPoint");
        Thumb = GameObject.FindWithTag("Thumb");
        gameObject.transform.parent = MidPoint.transform;
        transform.position = MidPoint.transform.position;


        gameObject.GetComponent<Renderer>().material.color = Color.cyan;
    }

    // Update is called once per frame
    void Update()
    {
        double[] Thumb = new double[3];
        double[] Index = new double[3];
        HapticNativePlugin.get_gripper_position(Thumb, Index);



        transform.position = MidPoint.transform.position + (UnityHaptics.DoubleToVect(Thumb) - UnityHaptics.GetToolPosition());
    }
}
