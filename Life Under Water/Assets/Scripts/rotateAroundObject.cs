using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateAroundObject : MonoBehaviour
{
    public GameObject rotateAroundThis;
    public float speed;
    private float angle;
    // Start is called before the first frame update
    void Start()
    {
        angle = 0;
    }

    // Update is called once per frame
    void Update()
    {
        angle += speed;
        transform.RotateAround(rotateAroundThis.transform.position, Vector3.up, angle);   
    }
}
