using System.Collections.Generic;
using UnityEngine;

public class PhoneManager : MonoBehaviour
{
    [SerializeField] private List<CharacterSheet> _characters;
    [SerializeField] private List<Application> _apps;
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
        for(int i = 0; i < _apps.Count; i++)
        {
            _apps[i].SetUp(_characters);
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
