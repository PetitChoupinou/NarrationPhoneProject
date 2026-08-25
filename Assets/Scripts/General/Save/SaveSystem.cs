using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using TMPro;

public static class SaveSystem
{
    public static void SaveDataToFile(string name,string storyID,bool photoTaken=false)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/phoneStory.save";
        FileStream stream=new FileStream(path, FileMode.OpenOrCreate);
        SaveData data=new SaveData(name, storyID,photoTaken);
        bf.Serialize(stream, data);
        stream.Close();
    }
    public static SaveData LoadDataFromFile()
    {
        string path = Application.persistentDataPath + "/phoneStory.save";
        if (!File.Exists(path))
        {
            return null;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);
        SaveData data = bf.Deserialize(stream) as SaveData;
        Debug.Log(data.Value());
       
        stream.Close();
        return data;
    }
}
