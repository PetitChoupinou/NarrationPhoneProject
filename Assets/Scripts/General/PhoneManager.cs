using System;
using System.Collections.Generic;
using TCG.Core.Dialogues;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;

public class PhoneManager : MonoBehaviour
{
    [SerializeField] private StoryAppSetup _setup;
    [SerializeField] private GameObject _appButtonPrefabs;
    [SerializeField] private GameObject _appButtonCanvas;
    [SerializeField] private GameObject _thoughtSystem;
    [SerializeField] private Network _network;
    public Dictionary<ApplicationType, GameObject> lockedApps=new Dictionary<ApplicationType, GameObject>();
    private List<BaseApplication> _apps=new List<BaseApplication>();
    private NotificationManager _notifManager;
    private AppDepth _currentDepth;
    private LocationData _currentLocation;
    [SerializeField] private ClockSystem _clockSystem;
    

    private static PhoneManager instance = null;
    public static PhoneManager Instance => instance;

    public AppDepth CurrentDepth { get => _currentDepth; }
    public LocationData CurrentLocation { get => _currentLocation;
        set {
            _network.ChangeReception(value.networkState);
            if (value.networkState != NetworkState.Bad && _currentLocation.networkState == NetworkState.Bad)
            {
                AppManager.Instance.GetApplication(ApplicationType.Messages).GetComponent<MessageApp>().NetworkIsGood();
            }
            _currentLocation = value;
        }
    }

    public ClockSystem ClockSystem { get => _clockSystem; set => _clockSystem = value; }

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
        inApp,
        deep
    };
    void Start()
    {
        _notifManager = NotificationManager.Instance;
        
        for (int i = 0; i < _setup.Applications.Count; i++)
        {
           BaseApplication app= Instantiate(_setup.Applications[i]).GetComponent<BaseApplication>();
            if (app.IsUnlocked)
            {
               AddApplication(_setup.Applications[i]);
            }
            else
            {
                lockedApps.Add(app._appType, _setup.Applications[i]);
            }
            Destroy(app.gameObject);

        }
        ClockSystem.SetUp(_setup);
    }
    public void CloseApps()
    {
        if(_currentDepth == AppDepth.deep)
        {
            ReturnApps();
        }
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
            case AppDepth.deep:
                ReturnApps();
                break;
            default:
                break;
        }
    }

    public void CreateThought(string thought)
    {
        if(_thoughtSystem != null)
        {
            _thoughtSystem.GetComponent<UITextTyper>().ReadText(thought);
        }
        else
        {
            Debug.LogError("Thought system is not assigned in PhoneManager.");
        }
    }

    public void ChangeLocation(LocationData location)
    {
        _currentLocation = location;
        Debug.Log($"Go to {location.locationName}");
        //Play location VFX
    }
    public void AddApplication(GameObject appli)
    {
        GameObject app = Instantiate(appli);
        BaseApplication application = app.GetComponent<BaseApplication>();
        if (_apps.Find(x => x._appType == application._appType))
        {
            Destroy(app);
            return;
        }
        _apps.Add(application);
        AppManager.Instance.addToApps(application);
        application.SetUp(_setup);
        GameObject button = Instantiate(_appButtonPrefabs, _appButtonCanvas.transform);
        button.GetComponent<AppButton>().Type = app.GetComponent<BaseApplication>()._appType;
        _notifManager.Buttons.Add(button.GetComponent<AppButton>());
    }
}
