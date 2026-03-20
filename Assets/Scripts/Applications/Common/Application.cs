using System.Collections.Generic;
using UnityEngine;

abstract public class Application : MonoBehaviour
{
    private Canvas _canvas;
     Canvas _phoneCanvas;
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _phoneCanvas = PhoneManager.Instance.gameObject.GetComponent<Canvas>();
    }
    abstract public void SetUp(List<CharacterSheet> characters);
    abstract public void CloseCurrent();

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
