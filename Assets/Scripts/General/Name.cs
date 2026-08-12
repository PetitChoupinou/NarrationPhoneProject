using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Audio.GeneratorInstance;

public class Name : MonoBehaviour
{
    [SerializeField] TMP_InputField nameField;
    SceneLoader _loader;
    SaveManager _saver;
    bool _touchKeyboardEnabled = false;
    private void Start()
    {
        _saver = SaveManager.instance;
        if (_saver == null) Destroy(gameObject);
        _loader = FindFirstObjectByType<SceneLoader>();
        if (_loader != null)
        {
            if (!_loader.IsNewStory) Destroy(gameObject);
        }
    }
    public void SetName()
    {
        _saver.SetName(nameField.text);
        _saver.SetStoryID(PhoneManager.Instance.Setup.Name);
        _saver.SaveData();

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