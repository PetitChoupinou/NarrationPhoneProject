using System.Collections.Generic;
using UnityEngine;



public class AppManager : MonoBehaviour
{
    private List<Application> _apps=new List<Application>();

    private static AppManager instance = null;
    public static AppManager Instance => instance;

    public List<Application> Apps { get => _apps;}

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
    public void addToApps(Application app)
    {
        _apps.Add(app);
    }

    public Application GetApplication(ApplicationType type)
    {
        return _apps.Find(app => app._appType == type);
    }

    public void OpenApp(ApplicationType apps)
    {
        GetApplication(apps).GetComponent<Canvas>().enabled = true;
        switch (apps)
        {
            case ApplicationType.Messages:
                break;
            case ApplicationType.Notes:

                break;
            case ApplicationType.Contacts:
                GetApplication(apps).GetComponent<ContactApp>().OnActivated();
                break;
            case ApplicationType.Clock:
                GetApplication(apps).GetComponent<ClockApp>().OnActivated();
                break;
            case ApplicationType.Map:
                GetApplication(apps).GetComponent<MapApp>().OnActivated();
                break;
            case ApplicationType.Camera:
                GetApplication(apps).GetComponent<CameraApp>().OnActivated();
                break;
        }
    }
}
