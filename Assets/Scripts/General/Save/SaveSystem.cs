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
    //To save player and story datas as files
    public static void SaveDataToFile(SaveData data)
    {
        string serizalizedData = JsonUtility.ToJson(data);
        File.WriteAllText(GetPath(data.name), serizalizedData);
    }

    /*public static SaveData LoadDataFromFile(string name, string parentName = "")
    {

    }*/

    /*public static void SaveDataToFile(string name,string storyID,bool photoTaken, List<string> dialoguesData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/phoneStory.save";
        FileStream stream=new FileStream(path, FileMode.OpenOrCreate);
        PlayerSaveData data=new PlayerSaveData(name, storyID,photoTaken, dialoguesData);
        bf.Serialize(stream, data);
        stream.Close();
    }*/
    public static SaveData LoadDataFromFile()
    {
        string path = Application.persistentDataPath + "/phoneStory.save";
        if (!File.Exists(path))
        {
            return null;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);
        PlayerSaveData data = bf.Deserialize(stream) as PlayerSaveData;
        Debug.Log(data.Value());
       
        stream.Close();
        return data;
    }

    public static string FindJsonFromName(string name, List<string> dataList)
    {
        foreach (var data in dataList)
        {
            string dataName = data.Split('\n')[0];
            if (dataName == name)
            {
                return data;
            }
        }
        return "";
    }
}
