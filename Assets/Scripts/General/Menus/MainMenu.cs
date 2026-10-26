using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        if(DarkModeManager.Instance != null) DarkModeManager.Instance.ApplyDarkMode();
        if (SoundManager.Instance.CurrentMus != SoundManager.Instance.MusicDictionary["AMB_MainMenu"].name)
        {
            SoundManager.Instance.PlayMusic("AMB_MainMenu");
        }
    }
}
