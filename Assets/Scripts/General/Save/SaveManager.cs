using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.Audio.GeneratorInstance;

public class SaveManager : MonoBehaviour
{
    static public SaveManager instance { get; private set; }
    public PlayerSaveData Save { get => save;}
    private StoryAppSetup _storySetup;

    private PlayerSaveData save;
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
        save = SaveSystem.LoadDataFromFile<PlayerSaveData>("save");
        if (save == null)
        {
            save = new PlayerSaveData("save");
            SaveSystem.SaveDataToFile(save);
        }
        //TODO:
        /*_storySetup=FindFirstObjectByType<SceneLoader>().RetrieveSavedStory(save.storyID);
        _storySetup.HasPhotoBeenTaken = save.photoTaken1;*/
    }

    public void SaveStory(StorySaveData data)
    {
        SaveSystem.SaveDataToFile(data, "Story");
    }
    public void SetName(string name)
    {
        save.playerName = name;
    }

#region Save/Load Dialogue
    public void SaveDialogues()
    {
        foreach(var character in _storySetup.Characters)
        {
            foreach(var dialogue in character.Dialogues)
            {
                SaveDialogue(dialogue);
            }
        }
    }


    
    public void SaveDialogue(DialogueData dialogueData)
    {
        //TODO:
        /*string newData = dialogueData.name + "\n" + JsonUtility.ToJson(dialogueData);
        string nameNewData = newData.Split('\n')[0];
        //Debug.Log(nameNewData);
        var foundData = save.dialoguesData.FirstOrDefault(x => x.Split('\n')[0] == nameNewData);

        if (foundData == null)
        {
            save.dialoguesData.Add(newData);
            //Debug.LogWarning("Not Found");
        }
        else
        {
            foundData = newData;
            //Debug.LogError("Found");
        }*/
    }

    public DialogueData LoadDialogue(string name)
    {
        return null;
        //TODO:
        /*string foundDialogue = SaveSystem.FindJsonFromName(name);
        string foundDialogueName = foundDialogue.Split("\n")[0];
        string foundDialogueData = foundDialogue.Split("\n")[1];
        DialogueData newData = ScriptableObject.CreateInstance<DialogueData>();

        JsonUtility.FromJsonOverwrite(foundDialogueData, newData);
        newData.name = foundDialogueName;
        return newData;*/
    }
    #endregion
}