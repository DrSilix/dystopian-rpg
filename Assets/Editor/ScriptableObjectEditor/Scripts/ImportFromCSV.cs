using Agent.Assembly;
using Codice.CM.SEIDInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ImportFromCSV
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
            WriteDataToSOs(type, soPath, folderPath);
        }

        return true;
    }


    private static void WriteDataToSOs(Type soType, string soPath, string folderPath)
    {
        string filePath = folderPath + soType.ToString() + ".csv";

        ScriptableObject[] scriptableObjects = AssemblyTypes.GetAllInstancesOfType(soPath, soType);
        List<SerializedProperty> properties = new List<SerializedProperty>();
        if (scriptableObjects.Length <= 0) return;

        Dictionary<string, ScriptableObject> soDictionary = new Dictionary<string, ScriptableObject>();
        foreach (ScriptableObject item in scriptableObjects)
        {
            soDictionary.Add(item.GetInstanceID().ToString(), item);
        }

        StreamReader reader = new StreamReader(filePath);
        string line = reader.ReadLine().Trim('"');
        string[] headers = line.Split("\",\"");

        while (!reader.EndOfStream) {
            line = reader.ReadLine().Trim('"');
            if (line.Length < 2)
            {
                Debug.Log("empty line length was " + line.Length);
                continue;
            }
            Debug.Log(line);
            string[] data = line.Split("\",\"");
            if (!soDictionary.ContainsKey(data[0]))
            {
                Debug.Log("no scriptable object associated with " + data[0]);
                continue;
            }
            SerializedObject serializedObject = new SerializedObject(soDictionary[data[0]]);
            SerializedProperty serializedProperty = serializedObject.GetIterator();
            serializedProperty.NextVisible(true);

            int i = 1;
            while (serializedProperty.NextVisible(false))
            {
                serializedProperty.SetUnderlyingValue(ConvertValueToType(data[i], serializedProperty));
                i++;
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        reader.Close();
    }

    private static object ConvertValueToType(string value, SerializedProperty property)
    {
        switch (property.GetUnderlyingValue())
        {
            case WeaponType weaponValue:
                return Enum.Parse(typeof(WeaponType), value);
            case FiringMode firingModeValue:
                return Enum.Parse(typeof(FiringMode), value);
            case Sprite spriteValue:
                return spriteValue;
            default:
                return Convert.ChangeType(value, property.GetUnderlyingType());
        }
    }



    /*private static void CreateAndPopulateFile(Type soType, string soPath, string folderPath)
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
        while (serializedProperty.NextVisible(false))
        {
            header.Append("\"" + serializedProperty.name + "\",");
        }
        header.Remove(header.Length - 1, 1);
        header.Append("\n");
        Debug.Log(header.ToString());
        writer.WriteLine(header.ToString());

        writer.Close();
        writer = new StreamWriter(filePath, true);

        foreach (var item in scriptableObjects)
        {
            serializedObject = new SerializedObject(item);
            serializedProperty = serializedObject.GetIterator();
            serializedProperty.NextVisible(true);

            StringBuilder entry = new StringBuilder();
            while (serializedProperty.NextVisible(false))
            {
                entry.Append("\"" + serializedProperty.GetUnderlyingValue().ToString() + "\",");
            }
            entry.Remove(entry.Length - 1, 1);
            entry.Append("\n");
            Debug.Log(entry.ToString());
            writer.WriteLine(entry.ToString());
        }
        writer.Close();
    }*/
}
