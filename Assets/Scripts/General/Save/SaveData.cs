using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SaveData
{
    public string name;
    public string storyID;
    public bool photoTaken1;

    [SerializeReference]
    public List<string> dialoguesData= new List<string>();
    public SaveData(string name, string storyID,bool photoTaken1, List<string> dialoguesData)
    {
        this.name = name;
        this.storyID = storyID;
        this.photoTaken1 = photoTaken1;
        this.dialoguesData = dialoguesData;
    }
    public string Value()
    {
        string ret=name + " " + storyID+" "+photoTaken1;
        return ret;
    }

    public string FindJsonFromName(string name)
    {
        foreach (var dialogue in dialoguesData)
        {
            string dialogueName = dialogue.Split('\n')[0];
            if(dialogueName == name)
            {
                return dialogue;
            }
        }
        return "";
    }
    
}
