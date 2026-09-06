using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerSaveData : SaveData
{
    public float energy;
    public PlayerSaveData(string name) : base(name)
    {

    }
}
