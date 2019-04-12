using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveTowardsCamera : MonoBehaviour
{
    public float speed;

    void Update()
    {
        float z = transform.position.z - speed * Time.deltaTime;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, z);
        transform.position = pos;
    }
}
