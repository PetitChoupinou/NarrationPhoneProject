using UnityEngine;
using static UnityEngine.Audio.GeneratorInstance;

public class SaveManager : MonoBehaviour
{
    static public SaveManager instance { get; private set; }
    public SaveData Save { get => save;}

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
        if (save == null) save = new SaveData("", "");
    }
    public void SetName(string name)
    {
        save.name = name;
    }
    public void SetStoryID(string ID)
    { 
        save.storyID = ID;
    }
    public void SaveData()
    {
        SaveSystem.SaveDataToFile(save.name, save.storyID);
    }
}

