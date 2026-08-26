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
    public SaveData(string name, string storyID,bool photoTaken1)
    {
        this.name = name;
        this.storyID = storyID;
        this.photoTaken1 = photoTaken1;
    }
    public string Value()
    {
        string ret=name + " " + storyID+" "+photoTaken1;
        return ret;
    }

    
}
