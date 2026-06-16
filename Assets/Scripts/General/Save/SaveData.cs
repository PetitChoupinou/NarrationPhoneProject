using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SaveData
{
    public string name;
    public string storyID;
    public SaveData(string name, string storyID)
    {
        this.name = name;
        this.storyID = storyID;
    }
}
