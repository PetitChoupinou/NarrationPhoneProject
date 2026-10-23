using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        if(DarkModeManager.Instance != null) DarkModeManager.Instance.ApplyDarkMode();
        SoundManager.Instance.PlayMusic("AMB_MainMenu");
    }
}
