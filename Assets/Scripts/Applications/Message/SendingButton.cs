using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendingButton : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] Image _sprite;
    [SerializeField] Color _enableColor;
    [SerializeField] Color _disableColor;
    private bool _enabled;
    List<Action> _actionsBuffer = new List<Action>();

    public bool Enabled { get => _enabled;}

    public void EnableButton(Action sendingAction)
    {
        
        _sprite.color = _enableColor;
        _button.onClick.AddListener(() => {
            DisableButton();
            sendingAction();
        });
        _enabled = true;
    }

    public void DisableButton()
    {
        _sprite.color = _disableColor;
        _button.onClick.RemoveAllListeners();
        _enabled = false;
    }
}
