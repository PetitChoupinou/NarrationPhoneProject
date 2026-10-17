using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void OnEnable()
    {
        if(DarkModeManager.Instance != null) DarkModeManager.Instance.ApplyDarkMode();
    }
}
