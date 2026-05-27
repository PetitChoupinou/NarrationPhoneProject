using System.Collections.Generic;
using UnityEngine;



public class AppManager : MonoBehaviour
{
    private List<BaseApplication> _apps=new List<BaseApplication>();
    private Header _header;
    private static AppManager instance = null;
    public static AppManager Instance => instance;

    public List<BaseApplication> Apps { get => _apps;}
    

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
    public void addToApps(BaseApplication app)
    {
        _apps.Add(app);
    }
    private void Start()
    {
        _header = FindFirstObjectByType<Header>();
    }
    public BaseApplication GetApplication(ApplicationType type)
    {
        return _apps.Find(app => app._appType == type);
    }

    public void OpenApp(ApplicationType apps)
    {
        GetApplication(apps).GetComponent<Canvas>().enabled = true;
        bool isLight = true ;
        bool needBg = false;
        switch (apps)
        {
            case ApplicationType.Messages:
                isLight = false;
                break;
            case ApplicationType.Notes:
                isLight = false;
                break;
            case ApplicationType.Contacts:
                GetApplication(apps).GetComponent<ContactApp>().OnActivated();
                isLight = false;
                break;
            case ApplicationType.Clock:
                GetApplication(apps).GetComponent<ClockApp>().OnActivated();
                isLight = false;
                break;
            case ApplicationType.Map:
                GetApplication(apps).GetComponent<MapApp>().OnActivated();
                needBg = true;
                break;
            case ApplicationType.Camera:
                GetApplication(apps).GetComponent<CameraApp>().OnActivated();
                isLight = false;
                break;
            case ApplicationType.Photos:
                isLight = false;
                break;
            case ApplicationType.Telephone:
                isLight = false;
                break;
            case ApplicationType.Internet:
                GetApplication(apps).GetComponent<InternetApp>().OnActivated();
                isLight = false;
                break;
        }
        _header.AppChangedUpdate(isLight,needBg);
    }
}
