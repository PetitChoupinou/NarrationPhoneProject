using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerSaveData : SaveData
{
    public TimeData lastAppQuit;
    public int currentEnergy;

    public PlayerSaveData(string name) : base(name)
    {
        lastAppQuit=new TimeData();
        currentEnergy = 100;
    }
    public string GetSaveTimeToString()
    {
        string minute = lastAppQuit.CurrentTime.Minute.ToString();
        if (lastAppQuit.CurrentTime.Minute < 10)
        {
            minute = $"0{lastAppQuit.CurrentTime.Minute}";
        }
        string hour = lastAppQuit.CurrentTime.Hour.ToString();
        if (lastAppQuit.CurrentTime.Hour < 10)
        {
            hour = $"0{lastAppQuit.CurrentTime.Hour}";
        }
        string day = lastAppQuit.CurrentTime.Day.ToString();
        if (lastAppQuit.CurrentTime.Day < 10)
        {
            day = $"0{lastAppQuit.CurrentTime.Day}";
        }
        string month = lastAppQuit.CurrentTime.Month.ToString();
        if (lastAppQuit.CurrentTime.Month < 10)
        {
            month = $"0{lastAppQuit.CurrentTime.Month}";
        }
        string year = lastAppQuit.CurrentTime.Year.ToString();

        return $"{day}/{month}/{year}, {hour}:{minute}";
    }
    public void SetLastAppQuit(TimeData timeData)
    {
        lastAppQuit = timeData;
    }
    public void SetCurrentEnergy(int energy)
    {
        currentEnergy = energy;
    }
}
