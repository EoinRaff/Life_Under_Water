using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public MeasureDepth measureDepth;
    public GameObject centerOfMass;
    public Camera wallCamera, floorCamera;
    public float heightFromFloor;

    private void Awake()
    {
        Singleton();
    }

    void Update()
    {
        CenterOfMassScreenToTransformPosition();
    }

    private void CenterOfMassScreenToTransformPosition()
    {
        Vector2 centerV2 = measureDepth.CenterOfMass;
        Vector3 position = floorCamera.ScreenToWorldPoint(new Vector3(centerV2.x, centerV2.y, heightFromFloor));
        position *= -1;
        centerOfMass.transform.position = position;
    }

    private void Singleton()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

}
