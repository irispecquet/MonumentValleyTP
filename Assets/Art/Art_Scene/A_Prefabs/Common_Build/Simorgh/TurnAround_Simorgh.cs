using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAround_Simorgh : MonoBehaviour
{
    public GameObject treeObjects;
    public float turnSpeed = 30f;


   
    private void FixedUpdate()
    {
        if (treeObjects != null)
        {
            treeObjects.transform.Rotate(new Vector3(0f, turnSpeed, 0f * Time.fixedDeltaTime));
        }
    }
}
