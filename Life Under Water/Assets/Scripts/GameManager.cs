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
        CenterOfMassScreenToTransformPosition();
    }

    private void OnGUI()
    {
        if (KinectServer.Instance == null || KinectServer.Instance.TriggerPoints == null)
            return;

        foreach (Vector2 point in KinectServer.Instance.TriggerPoints)
        {
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
        /*
        if (KinectServer.Instance == null || KinectServer.Instance.Data == null)
        {
            return;
        }
        */
        Vector2 centerV2 = KinectServer.Instance.Data.centerOfMass;
        Vector3 position = interactionCamera.ScreenToWorldPoint(new Vector3(centerV2.x, centerV2.y, CenterOfMassZPosition));
        position *= -1;
        centerOfMass.transform.position = position;
    }

    public void PlayHit()
    {
        gameObject.GetComponent<AudioSource>().Play();
    }

}
