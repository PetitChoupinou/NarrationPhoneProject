using UnityEngine;
using UnityEngine.UI;

public class StoryTab : MonoBehaviour
{
    private SceneLoader loader;
    [SerializeField] GameObject _button;
    [SerializeField] GameObject _confirmationPanel;

    void Start()
    {
        loader = FindAnyObjectByType<SceneLoader>();
        foreach(StoryAppSetup setup in loader.StoriesList)
        {
            GameObject button = Instantiate(_button, transform);
            button.GetComponent<StorySelectionButton>().SetUp(setup,this);
        }
    }
    public void OpenConfirmation(StoryAppSetup setup)
    {
        _confirmationPanel.GetComponent<Confirmation>().Setup = setup;
        _confirmationPanel.SetActive(true);
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
