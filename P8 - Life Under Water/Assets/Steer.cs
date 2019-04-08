using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steer : MonoBehaviour
{
    public Transform centerOfMass;

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.Rotate(centerOfMass.localPosition);
        Vector3 position = new Vector3();
        position.x = centerOfMass.position.x;
        position.z = 5; // -centerOfMass.position.x;
        position.y = 0;
        gameObject.transform.position = position;// = Quaternion.Euler(position);
    }
}
