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
        InitializeDisplays();
    }

    private static void InitializeDisplays()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }

    void Update()
    {
        CenterOfMassScreenToTransformPosition();
    }

    private void LateUpdate()
    {
        ObjectCleaner.DestroyObjectsAndClearList();
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
