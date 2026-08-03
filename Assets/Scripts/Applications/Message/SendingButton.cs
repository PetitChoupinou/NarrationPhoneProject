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
    List<Action> _actionsBuffer = new List<Action>();

    public void EnableButton(Action sendingAction)
    {
        
        _sprite.color = _enableColor;
        _button.onClick.AddListener(() => {
            DisableButton();
            sendingAction();
        });
    }

    public void DisableButton()
    {
        _sprite.color = _disableColor;
        _button.onClick.RemoveAllListeners();
    }
}
