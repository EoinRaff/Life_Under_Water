using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RectTrigger : MonoBehaviour
{
    [Range(0, 10)]
    public int sensitivity;
    public bool isTriggered = false;

    private Camera camera = null;
    private RectTransform rectTransform = null;
    private Image image = null;
    [SerializeField]
    private float lifetime;


    private void Awake()
    {

        camera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        if (GameManager.Instance.Interactive)
            MeasureDepth.OnTriggerPoints += OnTriggerPoints;
        else
            Destroy(gameObject, lifetime);
    }

    private void OnDestroy()
    {
        SceneController.Instance.GetComponent<AudioSource>().Play();
        DataManager.Trigger();
        SpawnTrash.trashCount--;
        if (GameManager.Instance.Interactive)
            MeasureDepth.OnTriggerPoints -= OnTriggerPoints;
    }

    private void OnTriggerPoints(List<Vector2> triggerPoints )
    {
        if (!enabled)
            return;

        int count = 0;
        foreach (Vector2 point in triggerPoints)
        {
            Vector2 flippedY = new Vector2(point.x, camera.pixelHeight - point.y);

            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, flippedY))
            {
                count++;
            }
        }
        if (count > sensitivity)
        {
            isTriggered = true;
            Destroy(rectTransform.gameObject);
        }
    }
}
