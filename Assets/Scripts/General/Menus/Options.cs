using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    SoundManager _soundManager;
    DarkModeManager _darkModeManager;
    SaveManager _saveManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Toggle sfxToggle;
    [SerializeField] Toggle musicToggle;
    [SerializeField] Toggle darkModeToggle;


    void Start()
    {
        _soundManager = SoundManager.Instance;
        _darkModeManager = DarkModeManager.Instance;
        sfxToggle.isOn = _soundManager.SfxVolume > 0;
        musicToggle.isOn = _soundManager.MusicVolume > 0;
        darkModeToggle.isOn = _darkModeManager.IsDarkMode;
    }

    private void OnEnable()
    {
        DarkModeManager.Instance.ApplyDarkMode();
    }
    public void SFXToggled(bool toggle)
    {
        _soundManager.SfxVolume=toggle ? 1 : 0;
    }
    public void MusicToggled(bool toggle)
    {
        _soundManager.MusicVolume = toggle ? 1 : 0;
    }

    public void DarkModeToggled(bool toggle)
    {
        _darkModeManager.SwitchMode(toggle);
    }
}
