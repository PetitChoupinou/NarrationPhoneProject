using UnityEngine;

public class StorySelectionButton : MonoBehaviour
{
    private SceneLoader _loader;
    private StoryAppSetup _setup;

    public void SetUp(StoryAppSetup setup)
    {
        _loader = FindFirstObjectByType<SceneLoader>();
        _setup = setup;
    }
    public void ChooseStory()
    {
        _loader.ChosenStory = _setup;
        _loader.LoadGameScene();
    }
}
