using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<Type> : MonoBehaviour where Type : MonoBehaviour
{
    public static Type Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = gameObject.GetComponent<Type>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
