using UnityEngine;

public class StoryTab : MonoBehaviour
{
    private SceneLoader loader;
    void Start()
    {
        loader = FindAnyObjectByType<SceneLoader>();
        foreach(StoryAppSetup setup in loader._story)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
