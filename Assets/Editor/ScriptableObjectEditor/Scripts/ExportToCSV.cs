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
        List<SerializedProperty> properties = new List<SerializedProperty>();
        if (scriptableObjects.Length <= 0) return;
        string filePath = folderPath + soType.ToString() + ".csv";
        Debug.Log(filePath);
        TextWriter writer = new StreamWriter(filePath, false);
        
        SerializedObject serializedObject = new SerializedObject(scriptableObjects[0]);
        SerializedProperty serializedProperty = serializedObject.GetIterator();
        serializedProperty.NextVisible(true);
        StringBuilder header = new StringBuilder();
        header.Append("\"GUID\",\"AssetName\",");
        while (serializedProperty.NextVisible(false))
        {
            header.Append("\"" + serializedProperty.name + "\",");
        }
        header.Remove(header.Length - 1, 1);
        Debug.Log(header.ToString());
        writer.WriteLine(header.ToString());

        //writer.Close();
        //writer = new StreamWriter(filePath, true);

        foreach (var item in scriptableObjects)
        {
            serializedObject = new SerializedObject(item);
            serializedProperty = serializedObject.GetIterator();
            serializedProperty.NextVisible(true);

            StringBuilder entry = new StringBuilder();
            entry.Append("\"" + item.GetInstanceID() + "\",");
            entry.Append("\"" + item.name + "\",");
            while (serializedProperty.NextVisible(false))
            {
                entry.Append("\"" + serializedProperty.GetUnderlyingValue().ToString() + "\",");
            }
            entry.Remove(entry.Length - 1, 1);
            Debug.Log(entry.ToString());
            writer.WriteLine(entry.ToString());
        }
        writer.Close();
    }

    /*
    serializedProperty = serializedObject.GetIterator();
    serializedProperty.NextVisible(true);
    DrawProperties(serializedProperty);
    */

    /*protected void DrawProperties(SerializedProperty property)
    {
        if (property.displayName == "Script") { GUI.enabled = false; }
        EditorGUILayout.PropertyField(property, true);
        GUI.enabled = true;

        EditorGUILayout.Space(40);

        while (property.NextVisible(false))
        {
            EditorGUILayout.PropertyField(property, true);
        }
    }*/
}
