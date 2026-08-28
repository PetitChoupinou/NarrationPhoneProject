using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerSaveData : SaveData
{

    public string playerName;
    [SerializeReference]
    public List<string> storiesData = new List<string>();

    public PlayerSaveData(string name) : base(name)
    {

    }

    

}
