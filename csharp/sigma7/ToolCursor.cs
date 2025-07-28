using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestToolCursor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.blue;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.SetPositionAndRotation(
                UnityHaptics.GetToolProxyPosition(),
				UnityHaptics.GetToolRotation());
    }
}
