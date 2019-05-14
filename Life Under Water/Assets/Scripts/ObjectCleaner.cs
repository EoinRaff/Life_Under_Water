using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectCleaner
{
    private static List<GameObject> ObjectsToDestroy = new List<GameObject>();

    public static void AddObjectToList(GameObject @object)
    {
        ObjectsToDestroy.Add(@object);
    }

    public static void DestroyObjectsAndClearList()
    {
        foreach (GameObject @object in ObjectsToDestroy)
        {
            Object.Destroy(@object);
        }
        ObjectsToDestroy.Clear();
    }
    
}
