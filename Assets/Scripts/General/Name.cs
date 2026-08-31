using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Audio.GeneratorInstance;

public class Name : MonoBehaviour
{
    [SerializeField] TMP_InputField nameField;
    SceneLoader _loader;
    SaveManager _saver;
    StorySaveData _storyData;
    bool _touchKeyboardEnabled = false;
    private void Start()
    {
        _saver = SaveManager.instance;
        if (_saver == null) Destroy(gameObject);
        _loader = FindFirstObjectByType<SceneLoader>();
        if (_loader != null)
        {
            _storyData = SaveManager.instance.LoadStory(_loader.CurrentStorySetup.Name);
            if (_storyData != null && !_storyData.isNewStory) Destroy(gameObject);
        }
    }
    public void SetName()
    {
        _storyData.isNewStory = false;
        _storyData.SetPlayerName(nameField.text);
        SaveManager.instance.SaveStory(_storyData);

        Destroy(gameObject);
    }
    public void OpenKeyboard()
    {
        if (!TouchScreenKeyboard.isSupported)
        {
            return;
        }
        TouchScreenKeyboard.Open(nameField.text);
    }
}