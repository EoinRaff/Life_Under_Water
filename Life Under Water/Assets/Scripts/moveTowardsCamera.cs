using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveTowardsCamera : MonoBehaviour
{
    public float speed;
    public bool trashIsland;
    public float trashIslandEndPosition;

    void Update()
    {
        float z = transform.position.z - speed * Time.deltaTime;
        if (trashIsland)
        {
            if (transform.position.z <= trashIslandEndPosition)
            {
                z = trashIslandEndPosition;
            }

        }
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, z);
        transform.position = pos;

        if (z <= -5)
        {
            Destroy(gameObject);
        }

    }
}
