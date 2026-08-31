using TMPro;
using UnityEngine;

public class StorySelectionButton : MonoBehaviour
{
    private SceneLoader _loader;
    private StoryAppSetup _setup;
    [SerializeField] private TMP_Text _text;
    [SerializeField] public string _buttonSfx;

    public void SetUp(StoryAppSetup setup)
    {
        _loader = FindFirstObjectByType<SceneLoader>();
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
    public void OnButtonClicked()
    {
        SoundManager.instance.PlaySound(_buttonSfx);
    }
}
