using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public static class DataManager
{
    public static int TriggerCount { get; private set; }
    public static List<Vector2> positionData = new List<Vector2>();

    public static char delimiter = ',';
    private static string triggerDataPath = Application.dataPath + "/TriggerData.csv";
    private static string posXDataPath = Application.dataPath + "/PositionXData.csv";
    private static string posYDataPath = Application.dataPath + "/PositionYData.csv";


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
        textWriter.Close();
        File.AppendAllText(triggerDataPath, DateTime.Now.ToString() + delimiter + TriggerCount.ToString() + Environment.NewLine);
        ClearData();
    }

    public static void PrintPositionData()
    {
        string outputx = DateTime.Now.ToString();
        string outputy = DateTime.Now.ToString();

        for (int i = 0; i < positionData.Count; i++)
        {
            outputx += delimiter + positionData[i].x.ToString();
            outputy += delimiter + positionData[i].y.ToString();
        }

        TextWriter textWriterX = new StreamWriter(posXDataPath, true);
        textWriterX.Close();
        TextWriter textWriterY = new StreamWriter(posYDataPath, true);
        textWriterY.Close();
        File.AppendAllText(posXDataPath, outputx + Environment.NewLine);
        File.AppendAllText(posYDataPath, outputy + Environment.NewLine);

        ClearData();
    }
}