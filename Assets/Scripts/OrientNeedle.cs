using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientNeedle : MonoBehaviour
{
    // Update is called once per frame

    public Transform milkyWay;
    void Update()
    {
        //Quaternion.RotateTowards(needle.transform.rotation, Quaternion.zero, rotationSpeed * Time.deltaTime);
        transform.LookAt(milkyWay.position);
    }
}
