using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private float startTime;
    [SerializeField]
    private float duration;
    //public Material water2plastic;
    public List<Renderer> renderers;

    // Start is called before the first frame update
    void Awake()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime + duration <= Time.time)
        {
            FadeToBlack();
            return;
        }
        float completionPercentage = Time.time / (startTime + duration);
        foreach (Renderer rend in renderers)
        {
            rend.material.SetFloat("_Blend", Mathf.Min(1, completionPercentage));
        }
    }

    private void FadeToBlack()
    {
            throw new NotImplementedException();
    }
}
