using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraApp : BaseApplication
{
    [SerializeField] private Image _thumbnail;
    private PhotoApp _photoApp;
    private Sprite _basePhoto;
    private bool _wasPhotoTaken;
    private PhoneManager _phoneManager;
    public override void CloseCurrent()
    {
       
    }

    public override void SetUp(StoryAppSetup setup)
    {
        _phoneManager=PhoneManager.Instance;
        _basePhoto =setup.BaseCameraPhoto;
        _wasPhotoTaken=setup.HasPhotoBeenTaken;
    }
    public override  void PostSetUp()
    {
        _photoApp = (PhotoApp)AppManager.Instance.GetApplication(ApplicationType.Photos);
        DateTime now = DateTime.Now;
        DateTime time = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        if (_wasPhotoTaken)
        {
            PhotoData newPhoto = new PhotoData
            {
                image = _basePhoto,
                year = time.Year,
                month = time.Month,
                day = time.Day,
                hour = time.Hour,
                minute = time.Minute
            };
            _photoApp.AddPhoto(newPhoto);
            _thumbnail.sprite = newPhoto.image;
        }

    }
    public void TakePhoto()
    {
        Debug.Log("*Clic* New photo");
        DateTime now = DateTime.Now;
        DateTime time = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        Sprite photo=_basePhoto;
        bool wasPhotoTaken = _wasPhotoTaken;
        if (AppManager.Instance.GetApplication(ApplicationType.Map))
        {

            photo = _phoneManager.CurrentLocation.photo;
            wasPhotoTaken = _phoneManager.CurrentLocation.hasPhotoBeenTaken;
        }
        if (wasPhotoTaken) 
        {
            _phoneManager.CreateThought(" j'ai déjà pris cette photo");
            return;
        }
        PhotoData newPhoto = new PhotoData
        {
            image = photo,
            year = time.Year,
            month = time.Month,
            day = time.Day,
            hour = time.Hour,
            minute = time.Minute
        };
        _photoApp.AddPhoto(newPhoto);
        _thumbnail.sprite = newPhoto.image;
        SaveManager.instance.SetListPhotoTaken(true);
        SaveManager.instance.SaveData();
    }

    public void OpenGallery()
    {
        //Aller chercher le dossier dans lequel est la photo
        PhoneManager.Instance.GetInApp();
        CloseApp();
        AppManager.Instance.OpenApp(ApplicationType.Photos);
        _photoApp.OpenLatestPhoto();


    }

    public void OnActivated()
    {
        _thumbnail.sprite = _photoApp.GetLatestPhoto().sprite;
    }
}

