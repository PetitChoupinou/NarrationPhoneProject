using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        if(DarkModeManager.Instance != null) DarkModeManager.Instance.ApplyDarkMode();
    }
}
