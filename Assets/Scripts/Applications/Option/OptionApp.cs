using UnityEngine;

public class OptionAPP : BaseApplication
{
    SoundManager _soundManager;
    SceneLoader _sceneLoader;

    public override void CloseCurrent()
    {

    }

    public override void SetUp(StoryAppSetup setup)
    {
        _soundManager = SoundManager.Instance;
        _sceneLoader = SceneLoader.Instance;
    }
    public void ToggleSound(bool toggle)
    {
        if(_soundManager)
        _soundManager.SfxVolume = toggle ? 1 : 0;
    }
    public void ToggleMusic(bool toggle)
    {
        if(_soundManager)
        _soundManager.MusicVolume = toggle ? 1 : 0;
    }
    public void SaveAndQuit()
    {
        _sceneLoader.LoadMenuScene();
    }
}
