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
    public Renderer rend;
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
            print("Finished");
            FadeToBlack();
            return;
        }
        float completionPercentage = Time.time / (startTime + duration);
        print(completionPercentage);
        print("BLEND: " + rend.material.GetFloat("_Blend"));
        rend.material.SetFloat("_Blend", Mathf.Min(1,completionPercentage));
    }

    private void FadeToBlack()
    {
        SceneManager scene;
        throw new NotImplementedException();

    }
}
