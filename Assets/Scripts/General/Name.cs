using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Audio.GeneratorInstance;

public class Name : MonoBehaviour
{
    [SerializeField] TMP_InputField nameField;
    SceneLoader _loader;
    SaveManager _saver;
    StoryAppSetup _setup;
    StorySaveData _storyData;
    bool _touchKeyboardEnabled = false;
    private void Start()
    {
        _saver = SaveManager.instance;
        if (_saver == null) Destroy(gameObject);
        _loader = FindFirstObjectByType<SceneLoader>();
    }
    private void OnEnable()
    {
        _setup = FindFirstObjectByType<Confirmation>().Setup;
    }
    public void SetName()
    {
        GetComponent<Canvas>().enabled = false;
        if (_loader == null) return;
        _loader.LoadGameScene(_setup, true);       
        _storyData = _saver.LoadStory(_setup.Name);
        _storyData.isNewStory = false;
        _storyData.SetPlayerName(nameField.text);
        _saver.SaveStory(_storyData);
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