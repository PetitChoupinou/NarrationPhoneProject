using UnityEngine;
using UnityEngine.UI;

public class OptionAPP : BaseApplication
{
    SoundManager _soundManager;
    SceneLoader _sceneLoader;
    DarkModeManager _darkModeManager;
    SaveManager _saveManager;
    [SerializeField] Toggle sfxToggle;
    [SerializeField] Toggle musicToggle;
    [SerializeField] Toggle darkModeToggle;

    public override void CloseCurrent()
    {

    }

    public override void SetUp(StoryAppSetup setup)
    {
        
        _soundManager = SoundManager.Instance;
        _sceneLoader = SceneLoader.Instance;
        _darkModeManager = DarkModeManager.Instance;
        sfxToggle.isOn = _soundManager.SfxVolume > 0;
        musicToggle.isOn = _soundManager.MusicVolume > 0;
        darkModeToggle.isOn = _darkModeManager.IsDarkMode;
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
    public void ToggleDarkMode(bool toggle)
    {

        if (_darkModeManager)
        _darkModeManager.SwitchMode(toggle);
    }

    public void SaveAndQuit()
    {
        _sceneLoader.LoadMenuScene();
    }
}
