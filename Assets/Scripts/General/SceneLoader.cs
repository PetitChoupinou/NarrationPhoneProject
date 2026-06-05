using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    #region Fields

    [SerializeField] private string _sceneToLoad = "Test";
    [SerializeField] private CanvasGroup _splashScreen;
    [SerializeField] private float _loadingTime=1.2f;

    private float _timer = 0;
    #endregion

    #region Methods

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void LoadTestScene()
    {
        StartCoroutine(LoadTestSceneAsync());
    }

    IEnumerator LoadTestSceneAsync()
    {
        AsyncOperation operation= SceneManager.LoadSceneAsync(_sceneToLoad);
        operation.allowSceneActivation = false;
        while( _timer<_loadingTime)
        {
            _timer += Time.deltaTime;
            _splashScreen.alpha = Mathf.Lerp(0.0f, 0.9f, _timer / _loadingTime);
                yield return null;
        }
        _timer = 0;
        operation.allowSceneActivation = true;
        while(!operation.isDone)//if the scene is not loaded yet we wait for it to be 
        {
            yield return null;
        }
        while (_timer < _loadingTime) // and then we fade out the splash scrren
        { 
            _timer += Time.deltaTime;
            _splashScreen.alpha = Mathf.Lerp(0.9f, 0.0f, _timer / _loadingTime);
            yield return null;
        }
        Destroy(gameObject);
        yield return null;
    }
}
#endregion
