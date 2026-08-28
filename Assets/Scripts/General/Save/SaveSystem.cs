using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{

    public static string GetPath(string name, string parentFolderName = "")
    {
        string path = Application.persistentDataPath + $"/{name}.json";
        if(!string.IsNullOrEmpty(parentFolderName))
        {
            string parentFolderPath = Application.persistentDataPath + $"/{parentFolderName}";
            if (!Directory.Exists(parentFolderPath))
            {
                Directory.CreateDirectory(parentFolderPath);
            }
            path = Application.persistentDataPath + $"/{parentFolderName}/{name}.json";
        }
        
        return path ;
    }

    public static void SaveDataToFile<T>(T data, string parentFolderName = "") where T : SaveData
    {
        string serizalizedData = JsonUtility.ToJson(data);
        File.WriteAllText(GetPath(data.name, parentFolderName), serizalizedData);
    }

    public static T LoadDataFromFile<T>(string name, string parentName = "") where T : SaveData
    {
        string path = GetPath(name, parentName);
        if(!File.Exists(path)) 
        {
            return null;
        }
        T data  = JsonUtility.FromJson<T>(path);
        return data;
    }

    
}
