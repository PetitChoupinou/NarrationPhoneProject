using System.Collections.Generic;
using UnityEngine;
public enum ApplicationType
{
    Messages,
    Contacts,
    Calendar,
    Notes,
    Clock,
    Settings,
    Photos,
    Map,
    Telephone,
    Camera
}
abstract public class Application : MonoBehaviour
{
    protected Canvas _canvas;
    Canvas _phoneCanvas;
    public ApplicationType _appType;
    private Header _header;
    [SerializeField] private Sprite _logo;

    public Sprite Logo { get => _logo;}

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }
    private void Start()
    {
        _phoneCanvas = PhoneManager.Instance.gameObject.GetComponent<Canvas>();
        _header = FindFirstObjectByType<Header>();

    }
    abstract public void SetUp(StoryAppSetup setup);
    abstract public void CloseCurrent();
    //indentedfield = serialzlizedIs=true:


    public void CloseApp()
    {
        if (_canvas.isActiveAndEnabled)
        {
            _header.AppChangedUpdate(true,false);
            _canvas.enabled = false;
            _phoneCanvas.enabled = true;
            PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.phone);
        }
    }
}
