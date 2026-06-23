using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    SoundManager _soundManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _soundManager = SoundManager.instance;
    }
    public void SFXToggled(bool toggle)
    {
        _soundManager.SfxVolume=toggle ? 1 : 0;
    }
    public void MusicToggled(bool toggle)
    {
        _soundManager.MusicVolume = toggle ? 1 : 0;
    }
}
