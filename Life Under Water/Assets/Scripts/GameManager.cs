using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject centerOfMass;
    public Camera interactionCamera;
    public float CenterOfMassZPosition = 10;

    private void Start()
    {
        DataManager.ClearData();
        InitializeDisplays();
    }

    void Update()
    {
        if (interactionCamera == null)
        {
            interactionCamera = Camera.main;
        }
        if (centerOfMass == null)
        {
            centerOfMass = GameObject.FindGameObjectWithTag("CoM");
        }
        print(Application.dataPath);
        CenterOfMassScreenToTransformPosition();
    }

    private void LateUpdate()
    {
        ObjectCleaner.DestroyObjectsAndClearList();
    }

    private static void InitializeDisplays()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }

    private void CenterOfMassScreenToTransformPosition()
    {
        Vector2 centerV2 = MeasureDepth.Instance.CenterOfMass;
        Vector3 position = interactionCamera.ScreenToWorldPoint(new Vector3(centerV2.x, centerV2.y, CenterOfMassZPosition));
        //position *= -1;
        centerOfMass.transform.position = position;
    }

}
