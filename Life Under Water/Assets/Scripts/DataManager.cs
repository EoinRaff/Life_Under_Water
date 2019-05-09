using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public static class DataManager
{
    public static int TriggerCount { get; private set; }
    public static List<Vector2> positionData;

    public static char delimiter = ',';
    private static string triggerDataPath = Application.dataPath + "/TriggerData.csv";
    private static string positionDataPath = Application.dataPath + "/PositionData.csv";
 
    
    public static void ClearData()
    {
        TriggerCount = 0;
        positionData.Clear();
    }

    public static void Trigger()
    {
        TriggerCount++;
    }

    public static void AddPosition(Vector2 position)
    {
        positionData.Add(position);
    }

    public static void PrintTriggerData()
    {
        TextWriter textWriter = new StreamWriter(triggerDataPath, true);
        File.AppendAllText(triggerDataPath, DateTime.Now.ToString() + delimiter+  TriggerCount.ToString());
        textWriter.Close();
        ClearData();
    }

    public static void PrintPositionData()
    {
        string output = DateTime.Now.ToString();
        for (int i = 0; i < positionData.Count; i++)
        {
            output += delimiter + positionData[i].ToString();
        }

        TextWriter textWriter = new StreamWriter(triggerDataPath, true);
        File.AppendAllText(triggerDataPath, output + Environment.NewLine);
        textWriter.Close();
        ClearData();
    }
}