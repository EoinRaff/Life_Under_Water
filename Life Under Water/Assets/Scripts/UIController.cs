using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public void LoadScene(bool Interactive)
    {
        print("PRESSED");
        GameManager.Instance.Interactive = Interactive;
        print("Loading Scene 1. Interative = " + Interactive);
        SceneManager.LoadScene("Scene1");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
