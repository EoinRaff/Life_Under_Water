using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public RectTransform centerOfMass, boundingBox;
    public Camera interactionCamera;

    private void Start()
    {
        InitializeDisplays();
    }

    void Update()
    {
        CenterOfMassToUI();
        BoundingBoxUI();
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

    private void CenterOfMassToUI()
    {
        if (KinectServer.Instance.Data == null)
            return;
        Vector2 centerV2 = KinectServer.Instance.Data.centerOfMass;
        centerOfMass.position = new Vector3(centerV2.x, -centerV2.y);
    }

    private void BoundingBoxUI()
    {
        if (KinectServer.Instance.Data == null)
            return;
        Vector2 newSize = KinectServer.Instance.Data.centerOfMass - 2*KinectServer.Instance.Data.bottomRight;
        boundingBox.sizeDelta = newSize;
    }

}
