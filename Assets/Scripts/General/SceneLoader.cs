using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    #region Fields

    [SerializeField] private string _sceneToLoad = "TestTexts";
    [SerializeField] private CanvasGroup _splashScreen;
    [SerializeField] private bool _isNewStory;
    [SerializeField] private float _loadingTime = 1.2f;
    [SerializeField] public List<StoryAppSetup> _story = new List<StoryAppSetup>();
    [SerializeField] public string _buttonSfx;
    private StoryAppSetup _chosenStory;

    private float _timer = 0;

    public StoryAppSetup ChosenStory { get => _chosenStory; set => _chosenStory = value; }
    public bool IsNewStory { get => _isNewStory; }
    #endregion

    #region Methods

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public StoryAppSetup RetrieveSavedStory(string savedStoryID)
    {
        _chosenStory = _story.Find(x => x.Name == savedStoryID);
        return _chosenStory;
    }

    public void LoadGameScene()
    {
        _isNewStory = true;
        ResetAllDialogues();
        SaveManager.instance.SaveDialogues();
        StartCoroutine(LoadGameSceneAsync());
    }
    public void LoadLastGameScene()
    {
        _isNewStory = false;
        StartCoroutine(LoadGameSceneAsync());
    }
    IEnumerator LoadGameSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneToLoad);
        operation.allowSceneActivation = false;
        while (_timer < _loadingTime)
        {
            _timer += Time.deltaTime;
            _splashScreen.alpha = Mathf.Lerp(0.0f, 1.0f, _timer / (_loadingTime - .2f));
            yield return null;
        }
        _timer = 0;
        operation.allowSceneActivation = true;
        while (!operation.isDone)//if the scene is not loaded yet we wait for it to be 
        {
            yield return null;
        }
        while (_timer < _loadingTime) // and then we fade out the splash scrren
        {
            _timer += Time.deltaTime;
            _splashScreen.alpha = Mathf.Lerp(1.0f, 0.0f, _timer / _loadingTime);
            yield return null;
        }
        yield return null;
    }

    private void ResetAllDialogues()
    {
        foreach (CharacterSheet character in _chosenStory.Characters)
        {
            foreach (DialogueData dialogue in character.Dialogues)
            {
                dialogue.ResetDialogue();
            }
        }
    }
    public void OnButtonClicked()
    {
        SoundManager.instance.PlaySound(_buttonSfx);
    }
    #endregion
}
