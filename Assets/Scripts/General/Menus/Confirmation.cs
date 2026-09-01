using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Confirmation : MonoBehaviour
{
    StoryAppSetup _setup;
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
        if (!SaveSystem.DoesFileExist(_setup.Name,"Story"))
        {
            _continueButton.SetActive(false);
            _restartButton.GetComponentInChildren<TMP_Text>().text = "Commencer l'histoire";
        }
        else
        {
            _continueButton.SetActive(true);
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
