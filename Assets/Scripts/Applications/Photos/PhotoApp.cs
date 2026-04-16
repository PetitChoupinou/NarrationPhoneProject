using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotoApp : Application
{
    [SerializeField] private GameObject _photoPanel;
    [SerializeField] private GameObject _photoPrefab;
    [SerializeField] private GameObject _returnButton;
    [SerializeField] private GameObject _buttonPanel;
    private GameObject _curentStoragePanel;
    [SerializeField] private TMP_Text _headerTxt;
    [SerializeField] private Image _photo;
    PhoneManager _phoneManager;
    private void Start()
    {
        _phoneManager = PhoneManager.Instance;
    }
    public override void CloseCurrent()
    {
        if (_phoneManager.CurrentDepth == PhoneManager.AppDepth.deep) 
        { 
            _phoneManager.ChangeDepth(PhoneManager.AppDepth.inApp);
        _photoPanel.SetActive(false);
        _returnButton.SetActive(false);
        _curentStoragePanel.SetActive(true);
        _headerTxt.text = "photo";
        }
        else if (_phoneManager.CurrentDepth == PhoneManager.AppDepth.inApp)
        {
            _phoneManager.ChangeDepth(PhoneManager.AppDepth.app);
            _photoPanel.SetActive(false);
            _returnButton.SetActive(false);
            _curentStoragePanel.SetActive(true);
            _headerTxt.text = "photo";
        }
    }
    public override void SetUp(StoryAppSetup setup)
    {
        
    }
}

