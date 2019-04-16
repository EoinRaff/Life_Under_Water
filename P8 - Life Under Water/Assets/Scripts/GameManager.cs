using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject centerOfMass;
    public Camera interactionCamera;
    public float heightFromGround; //Bad name. Not sure what this really is. 10 is a good value for it though

    private void Start()
    {
        InitializeDisplays();
    }

    void Update()
    {
        CenterOfMassScreenToTransformPosition();
    }

    private void OnGUI()
    {
        if (KinectServer.Instance.TriggerPoints == null)
            return;

        foreach (Vector2 point in KinectServer.Instance.TriggerPoints)
        {
            print(point);
            Rect rect = new Rect(point, new Vector2(10, 10));
            GUI.Box(rect, "");
        }
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
        Vector2 centerV2 = KinectServer.Instance.Data.centerOfMass;
        Vector3 position = interactionCamera.ScreenToWorldPoint(new Vector3(centerV2.x, centerV2.y, heightFromGround));
        position *= -1;
        centerOfMass.transform.position = position;
    }

}
