using UnityEngine;

public class SetReturnButton : MonoBehaviour
{
    private GameObject _currentPanel;
    private GameObject _returnPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PanelChanged(GameObject newPanel, GameObject returnPanel)
    {
        _currentPanel = newPanel;
        _returnPanel = returnPanel;
    }
    public void OnButtonPressed()
    {

    }
}
