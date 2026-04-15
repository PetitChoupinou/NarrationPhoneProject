using System.Collections.Generic;
using UnityEngine;

public class PhoneManager : MonoBehaviour
{
    [SerializeField] private StoryAppSetup _setup;
    [SerializeField] private GameObject _appButtonPrefabs;
    [SerializeField] private GameObject _appButtonCanvas;
     private List<Application> _apps=new List<Application>();
    private NotificationManager _notifManager;
    private AppDepth _currentDepth;

    private static PhoneManager instance = null;
    public static PhoneManager Instance => instance;


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
    public enum AppDepth
    {
        phone,
        app,
        inApp
    };
    void Start()
    {
        _notifManager = NotificationManager.Instance;
        for(int i = 0; i < _setup.Applications.Count; i++)
        {
            GameObject app = Instantiate(_setup.Applications[i]);
            _apps.Add(app.GetComponent<Application>());
            AppManager.Instance.addToApps(_apps[i]);
            _apps[i].SetUp(_setup);
            GameObject button = Instantiate(_appButtonPrefabs, _appButtonCanvas.transform);
            button.GetComponent<AppButton>().Type = app.GetComponent<Application>()._appType;
            _notifManager.Buttons.Add(button.GetComponent<AppButton>());
        }
    }
    public void CloseApps()
    {
        if (_currentDepth == AppDepth.inApp)
        {
            ReturnApps();
        }
        for (int i = 0; i < _apps.Count; i++)
        {
            _apps[i].CloseApp();
        }
    }
    public void ReturnApps()
    {
        for (int i = 0; i < _apps.Count; i++)
        {
            _apps[i].CloseCurrent();
        }
    }
    public void ChangeDepth(AppDepth depth)
    {
        _currentDepth = depth;
    }
    public void GetInApp()
    {
        GetComponent<Canvas>().enabled = false;
        ChangeDepth(AppDepth.app);
    }
    public void ReturnButton()
    {
        switch (_currentDepth)
        {
            case AppDepth.phone:
                break;
            case AppDepth.app:
                CloseApps();
                break;
            case AppDepth.inApp:
                ReturnApps();
                break;
             default:
                break;
        }
    }
}
