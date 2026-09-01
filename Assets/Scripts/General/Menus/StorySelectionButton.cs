using TMPro;
using UnityEngine;

public class StorySelectionButton : MonoBehaviour
{
    private SceneLoader _loader;
    private StoryAppSetup _setup;
    private StoryTab _storySelectionPanel;
    [SerializeField] private TMP_Text _text;
    [SerializeField] public string _buttonSfx;

    public void SetUp(StoryAppSetup setup,StoryTab storySelection)
    {
        _loader = FindFirstObjectByType<SceneLoader>();
        _storySelectionPanel = storySelection;
        _setup = setup;
        _text.text = setup.Name;
    }
    public void ContinueStory()
    {
        _loader.LoadGameScene(_setup, false);

    }

    public void StartNewStory()
    {
        _loader.LoadGameScene(_setup, true);

    }
    public void OpenConfirmation()
    {
        _storySelectionPanel.OpenConfirmation(_setup);
    }
    public void OnButtonClicked()
    {
        SoundManager.instance.PlaySound(_buttonSfx);
    }

}
