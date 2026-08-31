using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class StorySaveData : SaveData
{
    public string playerName;
    public string storyID;
    public bool photoTaken1;
    public TimeData dateOfSave;
    public bool isNewStory;

    [SerializeReference]
    public List<string> dialoguesData= new List<string>();

    public StorySaveData(string name) : base(name)
    {
        dateOfSave = new TimeData();
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
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

    public string GetSaveTimeToString()
    {
        string minute = dateOfSave.CurrentTime.Minute.ToString();
        if (dateOfSave.CurrentTime.Minute < 10)
        {
            minute = $"0{dateOfSave.CurrentTime.Minute}";
        }
        string hour = dateOfSave.CurrentTime.Hour.ToString();
        if (dateOfSave.CurrentTime.Hour < 10)
        {
            hour = $"0{dateOfSave.CurrentTime.Hour}";
        }
        string day = dateOfSave.CurrentTime.Day.ToString();
        if(dateOfSave.CurrentTime.Day < 10)
        {
            day = $"0{dateOfSave.CurrentTime.Day}";
        }
        string month = dateOfSave.CurrentTime.Month.ToString();
        if (dateOfSave.CurrentTime.Month < 10)
        {
            month = $"0{dateOfSave.CurrentTime.Month}";
        }
        string year = dateOfSave.CurrentTime.Year.ToString();

        return $"{day}/{month}/{year}, {hour}:{minute}";
    }
}


