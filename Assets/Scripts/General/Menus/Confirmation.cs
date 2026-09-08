using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Confirmation : MonoBehaviour
{
    StoryAppSetup _setup;
    SaveManager _saveManager;
    SceneLoader _loader;
    [SerializeField]GameObject _restartButton;
    [SerializeField]GameObject _continueButton;

    public StoryAppSetup Setup { get => _setup; set => _setup = value; }

    private void Start()
    {
        _loader = FindFirstObjectByType<SceneLoader>();
    }
    private void OnEnable()
    {
        if (_saveManager ==null) _saveManager = SaveManager.instance;
        StorySaveData save = _saveManager.LoadStory(_setup.Name);
        if (!SaveSystem.DoesFileExist(_setup.Name,"Story"))
        {
            _continueButton.SetActive(false);
            _restartButton.GetComponentInChildren<TMP_Text>().text = "Commencer l'histoire";
        }
        else
        {
            _continueButton.SetActive(true);
            _continueButton.GetComponentInChildren<TMP_Text>().text = "Continuer l'histoire : " + save.dateOfSave.CurrentTime.ToString();
            _restartButton.GetComponentInChildren<TMP_Text>().text = "Recommencer l'histoire à zéro";
        }
    }
    public void Restart()
    {
            _loader.CurrentStorySetup = _setup;
            _loader.LoadGameScene(_setup,true);
    }

    public void Resume()
    {
        _loader.CurrentStorySetup = _setup;
        _loader.LoadGameScene(_setup, false);
    }
}
