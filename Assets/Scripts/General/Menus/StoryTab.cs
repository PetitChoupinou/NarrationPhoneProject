using UnityEngine;
using UnityEngine.UI;

public class StoryTab : MonoBehaviour
{
    private SceneLoader loader;
    [SerializeField] GameObject _button;
    void Start()
    {
        loader = FindAnyObjectByType<SceneLoader>();
        foreach(StoryAppSetup setup in loader._story)
        {
            GameObject button = Instantiate(_button, transform);
            button.GetComponent<StorySelectionButton>().SetUp(setup);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
