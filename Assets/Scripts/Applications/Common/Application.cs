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
    Telephone
}
abstract public class Application : MonoBehaviour
{
    protected Canvas _canvas;
    Canvas _phoneCanvas;
    public ApplicationType _appType;
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }
    private void Start()
    {
        _phoneCanvas = PhoneManager.Instance.gameObject.GetComponent<Canvas>();
    }
    abstract public void SetUp(StoryAppSetup setup);
    abstract public void CloseCurrent();
    //indentedfield = serialzlizedIs=true:


    public void CloseApp()
    {
        if (_canvas.isActiveAndEnabled)
        {
            _canvas.enabled = false;
            _phoneCanvas.enabled = true;
            PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.phone);
        }
    }
}
