using Agent.Assembly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class ExportToCSV
{
    public static bool Go()
    {
        Type[] scriptableObjectTypes = AssemblyTypes.GetAllTypes();
        string soPath = "Assets";
        string folderPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../ScriptableObjectsCSVs/"));
        Debug.Log(folderPath);
        foreach (var type in scriptableObjectTypes)
        {
            Debug.Log(type.ToString());
            CreateAndPopulateFile(type, soPath, folderPath);
        }
        return true;
    }

    private static void CreateAndPopulateFile(Type soType, string soPath, string folderPath)
    {
        ScriptableObject[] scriptableObjects = AssemblyTypes.GetAllInstancesOfType(soPath, soType);
        if (scriptableObjects.Length <= 0) return;
        string filePath = folderPath + soType.ToString() + ".csv";
        Debug.Log(filePath);
        TextWriter writer = new StreamWriter(filePath, false);
        
        SerializedObject serializedObject = new(scriptableObjects[0]);
        SerializedProperty serializedProperty = serializedObject.GetIterator();
        serializedProperty.NextVisible(true);
        StringBuilder header = new();
        header.Append("\"GUID\",\"AssetName\",");
        while (serializedProperty.NextVisible(false))
        {
            header.Append($"\"{serializedProperty.name}\",");
        }
        header.Remove(header.Length - 1, 1);
        Debug.Log(header.ToString());
        writer.WriteLine(header.ToString());

        foreach (var item in scriptableObjects)
        {
            serializedObject = new SerializedObject(item);
            serializedProperty = serializedObject.GetIterator();
            serializedProperty.NextVisible(true);

            StringBuilder entry = new();
            entry.Append($"\"{item.GetInstanceID()}\",");
            entry.Append($"\"{item.name}\",");
            while (serializedProperty.NextVisible(false))
            {
                entry.Append($"\"{serializedProperty.GetUnderlyingValue().ToString()}\",");
            }
            entry.Remove(entry.Length - 1, 1);
            Debug.Log(entry.ToString());
            writer.WriteLine(entry.ToString());
        }
        writer.Close();
    }
}
