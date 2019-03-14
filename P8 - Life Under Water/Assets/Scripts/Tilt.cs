using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilt : MonoBehaviour
{
    public Transform centerOfMass;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.Rotate(centerOfMass.localPosition);
        Vector3 rotation = new Vector3();
        rotation.x = centerOfMass.position.z;
        rotation.z = -centerOfMass.position.x;
        rotation.y = 0;

        gameObject.transform.rotation = Quaternion.Euler(rotation);
    }
}
