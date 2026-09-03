using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    #region Fields
    [SerializeField] private string _sceneToLoad = "TestTexts";
    [SerializeField] private string _menuScene = "MenuScene";
    [SerializeField] private CanvasGroup _splashScreen;
    [SerializeField] private float _loadingTime = 1.2f;
    [SerializeField] private List<StoryAppSetup> _storiesList = new List<StoryAppSetup>();
    [SerializeField] public string _buttonSfx;
    [SerializeField] private StoryAppSetup currentStorySetup;

    private float _timer = 0;

    public List<StoryAppSetup> StoriesList { get => _storiesList; set => _storiesList = value; }
    public StoryAppSetup CurrentStorySetup { get => currentStorySetup; set => currentStorySetup = value; }
    #endregion

    #region Methods

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public StoryAppSetup GetStorySetup(string storyName)
    {
        StoryAppSetup storySetup = _storiesList.Find(x => x.Name == storyName);
        return storySetup;
    }

   public void LoadMenuScene()
    {
        StartCoroutine(LoadGameSceneAsync(_menuScene));
    }
    public void LoadGameScene(StoryAppSetup storySetup, bool isStartingAgain)
    {
        if(isStartingAgain)
        {
            StorySaveData newData = new StorySaveData(storySetup.Name);
            newData.isNewStory = true;
            SaveManager.instance.SaveStory(newData);
            SaveManager.instance.SaveDialogues(newData.name);
        }
        currentStorySetup = storySetup;
        /*ResetAllDialogues();
        SaveManager.instance.SaveDialogues();*/
        StartCoroutine(LoadGameSceneAsync(_sceneToLoad));
    }

    IEnumerator LoadGameSceneAsync(string sceneToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
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

    private void ResetAllDialogues(string storyName)
    {
        StoryAppSetup storySetup = GetStorySetup(storyName);
        foreach (CharacterSheet character in storySetup.Characters)
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
