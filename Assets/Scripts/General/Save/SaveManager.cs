using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Audio.GeneratorInstance;

public class SaveManager : MonoBehaviour
{
    static public SaveManager instance { get; private set; }
    public SaveData Save { get => save;}
    private StoryAppSetup _storySetup;

    private SaveData save;
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
        save = SaveSystem.LoadDataFromFile();
        if (save == null) save = new SaveData("", "",false);
        _storySetup=FindFirstObjectByType<SceneLoader>().RetrieveSavedStory(save.storyID);
        _storySetup.HasPhotoBeenTaken = save.photoTaken1;
    }
    public void SetName(string name)
    {
        save.name = name;
    }
    public void SetStoryID(string ID)
    { 
        save.storyID = ID;
    }
    public void SetListPhotoTaken(bool photoTaken)
    {
        save.photoTaken1 = photoTaken;
    }

    public void SaveData()
    {
        SaveSystem.SaveDataToFile(save.name, save.storyID,save.photoTaken1);
        SaveDialogues();
    }

    public void SaveDialogues()
    {
        foreach(var character in _storySetup.Characters)
        {
            foreach(var dialogue in character.Dialogues)
            {
                SaveSystem.SaveDialogue(save, dialogue);
            }
        }
    }

    public void SaveDialogue(DialogueData dialogue)
    {
        SaveSystem.SaveDialogue(save, dialogue);
    }
}

