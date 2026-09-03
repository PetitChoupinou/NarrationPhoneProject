using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static public SaveManager instance { get; private set; }
    public PlayerSaveData Save { get => _save;}

    private PlayerSaveData _save;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        _save = SaveSystem.LoadDataFromFile<PlayerSaveData>("save");
        if (_save == null)
        {
            _save = new PlayerSaveData("save");
            SaveSystem.SaveDataToFile(_save);
        }
    }

    public void SavePlayerData()
    {
        SaveSystem.SaveDataToFile(_save);
    }

    public void SaveStory(StorySaveData data)
    {
        data.dateOfSave.CurrentTime = DateTime.Now;
        data.dateOfSave.SetTimeFromCurrentTime();
        SaveSystem.SaveDataToFile(data, "Story");
    }

    public StorySaveData LoadStory(string storyName)
    {
        StorySaveData storySaveData = SaveSystem.LoadDataFromFile<StorySaveData>(storyName, "Story");
        if (storySaveData == null)
        {
            Debug.LogError("Failed to get story data at: " + SaveSystem.GetPath(storyName, "Story"));
            
            return null;
        }
        storySaveData.dateOfSave.SetCurrentTime();
        return storySaveData;
    }

    public string GetCurrentStoryPlayerName()
    {
        StorySaveData currentStoryData = LoadStory(FindFirstObjectByType<SceneLoader>().CurrentStorySetup.Name);
        return currentStoryData.playerName;
    }

    #region Save/Load Dialogue
    public void SaveDialogues(string storyName)
    {
        StoryAppSetup storySetup = FindFirstObjectByType<SceneLoader>().GetStorySetup(storyName);
        /*StorySaveData storySaveData = LoadStory(storyName);
        storySaveData.dialoguesData.Clear();
        SaveStory(storySaveData);*/
        foreach (var character in storySetup.Characters)
        {
            foreach(var dialogue in character.Dialogues)
            {
                SaveDialogue(dialogue, storyName);
            }
        }
    }


    
    public void SaveDialogue(DialogueData dialogueData, string storyName)
    {
        StorySaveData storySaveData = LoadStory(storyName);
        string newData = dialogueData.name + "\n" + JsonUtility.ToJson(dialogueData);
        string nameNewData = newData.Split('\n')[0];
        //Debug.Log(nameNewData);
        var foundData = storySaveData.dialoguesData.FirstOrDefault(x => x.Split('\n')[0] == nameNewData);
        if (foundData == null)
        {
            storySaveData.dialoguesData.Add(newData);
            //Debug.LogWarning("Not Found");
        }
        else
        {
            int index = storySaveData.dialoguesData.IndexOf(foundData);
            storySaveData.dialoguesData[index] = newData;
            
            //Debug.LogError("Found");
        }
        SaveStory(storySaveData);
    }


    public DialogueData LoadDialogue(string name, string storyName)
    {
        StorySaveData storySaveData = LoadStory(storyName);
        string foundDialogue = storySaveData.FindJsonFromName(name, storySaveData.dialoguesData);
        string foundDialogueName = foundDialogue.Split("\n")[0];
        string foundDialogueData = foundDialogue.Split("\n")[1];
        DialogueData newData = ScriptableObject.CreateInstance<DialogueData>();

        JsonUtility.FromJsonOverwrite(foundDialogueData, newData);
        newData.name = foundDialogueName;
        return newData;
    }
    #endregion

    #region Save/Load Location Photo

    public void SaveLocationPhoto(string locationName, PhotoData photoData, string storyName)
    {
        StorySaveData storyData = LoadStory(storyName);
        DateTime time = new DateTime(photoData.year, photoData.month, photoData.day, photoData.hour, photoData.minute, 0);
        LocationPhotoData locationPhotoData = new LocationPhotoData(time, photoData.image);

        string newData = locationName + "\n" + JsonUtility.ToJson(locationPhotoData);
        string nameNewData = newData.Split('\n')[0];
        var foundData = storyData.locationPhotoData.FirstOrDefault(x => x.Split('\n')[0] == nameNewData);
        if (foundData == null)
        {
            storyData.locationPhotoData.Add(newData);
        }
        else
        {
            int index = storyData.locationPhotoData.IndexOf(foundData);
            storyData.locationPhotoData[index] = newData;
        }
        SaveStory(storyData);
        
    }

    public LocationPhotoData LoadLocationPhoto(string locationName, string storyName)
    {
        StorySaveData storyData = LoadStory(storyName);
        string foundLocationData = storyData.FindJsonFromName(locationName, storyData.locationPhotoData);
        if(foundLocationData == "")
        {
            return null;
        }
        string foundLocationDataName = foundLocationData.Split("\n")[0];
        string foundLocationDataJson = foundLocationData.Split("\n")[1];

        LocationPhotoData newData = JsonUtility.FromJson<LocationPhotoData>(foundLocationDataJson);
        newData.locationName = foundLocationDataName;
        return newData;
    }

    #endregion
}