using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SceneController : Singleton<SceneController>
{
    private float startTime;
    [SerializeField]
    private float duration;
    //public Material water2plastic;
    public List<Renderer> renderers;
    private bool started = false;

    [SerializeField]
    private AudioClip scene1Audio, scene2Audio;
    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            startTime = Time.time;
            started = true;
        }
        switch (SceneManager.GetActiveScene().name)
        {
            case "Scene1":

                break;
            case "Scene2":
                float completionPercentage = Time.time / (startTime + duration);
                if (renderers.Count < 1 || renderers[0] == null)
                {
                    renderers.Clear();
                    GameObject[] objects = GameObject.FindGameObjectsWithTag("water");

                    for (int i = 0; i < objects.Length; i++)
                    {
                        renderers.Add(objects[i].GetComponent<MeshRenderer>());
                    }
                }
                foreach (Renderer rend in renderers)
                {
                    rend.material.SetFloat("_Blend", Mathf.Min(1, Mathf.Max(0.2f, completionPercentage)));
                }
                break;
            default:
                break;
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                PrintDataAndLoadScene();
            }
        }

    }

    private void FixedUpdate()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Scene1":

                break;
            case "Scene2":
                DataManager.AddPosition(new Vector2(MeasureDepth.Instance.CenterOfMass.x, MeasureDepth.Instance.CenterOfMass.y));
                break;
            default:
                break;
        }
    }

    private void PrintDataAndLoadScene()
    {
        string nextScene = "";
        switch (SceneManager.GetActiveScene().name)
        {
            case "Scene1":
                DataManager.PrintTriggerData();
                nextScene = "Scene2";
                break;
            case "Scene2":
                DataManager.PrintPositionData();
                nextScene = "Scene 0";
                break;
            default:
                break;
        }
        SceneManager.LoadScene(nextScene);
        started = false;
        //fade to black and load scene async via corountine

    }

    private void FadeToBlack()
    {
            throw new NotImplementedException();
    }
}
