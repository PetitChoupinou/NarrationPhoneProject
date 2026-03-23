using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;



public class AppManager : MonoBehaviour
{
    [SerializeField] private List<Application> _apps;

    private static AppManager instance = null;
    public static AppManager Instance => instance;

    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public Application GetApplication(ApplicationType type)
    {
        return _apps.Find(app => app._appType == type);
    }
}
