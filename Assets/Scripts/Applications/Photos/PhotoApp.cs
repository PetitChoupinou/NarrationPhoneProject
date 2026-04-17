using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class PhotoApp : Application
{
    [SerializeField] private GameObject _photoPanel;
    [SerializeField] private GameObject _photoPrefab;
    [SerializeField] private GameObject _returnButton;
    [SerializeField] private GameObject _buttonPanel;
    [SerializeField] private GameObject _buttonPrefab;
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
        _curentStoragePanel.SetActive(true);
        _headerTxt.text = _curentStoragePanel.GetComponent<PhotoPreview>().Title;
        }
        else if (_phoneManager.CurrentDepth == PhoneManager.AppDepth.inApp)
        {
            _phoneManager.ChangeDepth(PhoneManager.AppDepth.app);
            _curentStoragePanel.SetActive(false);
            _returnButton.SetActive(false);
            _buttonPanel.SetActive(true);
            _headerTxt.text = "photo";
            _curentStoragePanel=null;
        }
    }
    public override void SetUp(StoryAppSetup setup)
    {
        List<PhotoPreviews> photos = setup.Photos;
        foreach (PhotoPreviews photo in photos) 
        {
        
            GameObject button = Instantiate(_buttonPrefab, _buttonPanel.transform);
            GameObject photoPrev = Instantiate(_photoPrefab, transform);
            button.GetComponent<InAppButton>().SetUp(photo.title, photoPrev, _returnButton);
            var photoPreview = photoPrev.GetComponent<PhotoPreview>();
            photoPreview.SetUp(photo, _photo,_headerTxt,_returnButton,_photoPanel);
                photoPrev.SetActive(false);
        }
    }
}

