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
    private GameObject _currentStoragePanel;
    [SerializeField] private TMP_Text _headerTxt;
    [SerializeField] private Image _photo;
    private string _baseFolder;
    private List<PhotoPreview> photoPreviews=new List<PhotoPreview>();
    PhoneManager _phoneManager;

    public GameObject CurrentStoragePanel { get => _currentStoragePanel; set => _currentStoragePanel = value; }

    public override void CloseCurrent()
    {
        if (_phoneManager.CurrentDepth == PhoneManager.AppDepth.deep) 
        { 
            _phoneManager.ChangeDepth(PhoneManager.AppDepth.inApp);
        _photoPanel.SetActive(false);
        _currentStoragePanel.SetActive(true);
            _currentStoragePanel.GetComponent<PhotoPreview>().StoragePanel.SetActive(true);
        _headerTxt.text = _currentStoragePanel.GetComponent<PhotoPreview>().Title;
        }
        else if (_phoneManager.CurrentDepth == PhoneManager.AppDepth.inApp)
        {
            _phoneManager.ChangeDepth(PhoneManager.AppDepth.inApp);
            _currentStoragePanel.SetActive(false);
            _returnButton.SetActive(false);
            _buttonPanel.SetActive(true);
            _headerTxt.text = "photo";
            _currentStoragePanel=null;
        }
    }
    public override void SetUp(StoryAppSetup setup)
    {
        _phoneManager = PhoneManager.Instance;
        List<PhotoPreviews> photos = setup.Photos;
        _baseFolder = photos[0].title;
        foreach (PhotoPreviews photo in photos) 
        {
        
            GameObject button = Instantiate(_buttonPrefab, _buttonPanel.transform);
            GameObject photoPrev = Instantiate(_photoPrefab, transform);
            button.GetComponent<InAppButton>().SetUp(photo.title, photoPrev, _returnButton);
            var photoPreview = photoPrev.GetComponent<PhotoPreview>();
            photoPreview.SetUp(photo, _photo,_headerTxt,_returnButton,_photoPanel);
                photoPrev.SetActive(false);
            photoPreviews.Add(photoPreview);
        }
    }
    public void AddPhoto(PhotoData photo)
    {
        PhotoPreview searchedPreview = photoPreviews.Find(x => x.Title == _baseFolder);
        searchedPreview.AddPhoto(photo, _photo);
    }

}

