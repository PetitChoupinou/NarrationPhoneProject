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
        string foundDialogue = storySaveData.FindJsonFromName(name);
        string foundDialogueName = foundDialogue.Split("\n")[0];
        string foundDialogueData = foundDialogue.Split("\n")[1];
        DialogueData newData = ScriptableObject.CreateInstance<DialogueData>();

        JsonUtility.FromJsonOverwrite(foundDialogueData, newData);
        newData.name = foundDialogueName;
        return newData;
    }

    
    #endregion
}