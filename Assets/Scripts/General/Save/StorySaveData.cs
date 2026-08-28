using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class StorySaveData : SaveData
{
    
    public string storyID;
    public bool photoTaken1;

    [SerializeReference]
    public List<string> dialoguesData= new List<string>();

    public StorySaveData(string name) : base(name)
    {

    }

    public string FindJsonFromName(string name)
    {
        foreach (var dialogue in dialoguesData)
        {
            string dialogueName = dialogue.Split('\n')[0];
            if (dialogueName == name)
            {
                return dialogue;
            }
        }
        return "";
    }
}
