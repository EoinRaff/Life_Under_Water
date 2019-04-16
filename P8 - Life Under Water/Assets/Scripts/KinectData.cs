using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KinectData
{
    public int kinectID;
    public Vector2 offset;
    public Vector2 centerOfMass;
    public Vector2 bottomRight, topLeft;
    public Vector2[] triggerPoints;

    public KinectData(int arraysize)
    {
        this.kinectID = 1;
        this.offset = Vector2.zero;
        triggerPoints = new Vector2[arraysize];
    }
}