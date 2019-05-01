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

    private void Awake()
    {
        MeasureDepth.OnTriggerPoints += OnTriggerPoints;

        camera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void OnDestroy()
    {
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
                // could put other events or functionaility here
                // make trigger in, trigger out systems.
                count++;
                Destroy(rectTransform.gameObject);
            }
        }
        if (count > sensitivity)
        {
            isTriggered = true;
            image.color = Color.red;
        }
    }
}
