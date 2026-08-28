using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerSaveData : SaveData
{
    [SerializeReference]
    public List<string> storiesData = new List<string>();

    public PlayerSaveData(List<string> storiesData)
    {
        this.storiesData = storiesData;
    }

}
