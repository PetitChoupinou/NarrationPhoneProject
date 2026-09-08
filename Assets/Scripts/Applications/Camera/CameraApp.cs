using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;


public class CameraApp : BaseApplication
{
    [SerializeField] private Image _thumbnail;
    private PhotoApp _photoApp;
    private MapApp _mapApp;
    private Sprite _basePhoto;
    private PhoneManager _phoneManager;
    public override void CloseCurrent()
    {
       
    }

    public override void SetUp(StoryAppSetup setup)
    {
        _phoneManager=PhoneManager.Instance;
        _basePhoto =setup.BaseCameraPhoto;
        _photoApp = AppManager.Instance.GetApplication(ApplicationType.Photos).GetComponent<PhotoApp>();
        _mapApp = AppManager.Instance.GetApplication(ApplicationType.Map).GetComponent<MapApp>();
    }

    public override void PostSetUp()
    {
        StoryAppSetup setup = _phoneManager.Setup;
        foreach (var location in _mapApp.locations)
        {
            LocationPhotoData photoSavedData = SaveManager.instance.LoadLocationPhoto(location.Data.locationName, setup.Name);
            if (photoSavedData != null)
            {
                photoSavedData.datePhoto.SetCurrentTime();
                PhotoData photoData = new PhotoData()
                {
                    image = photoSavedData.photo,
                    year = photoSavedData.datePhoto.CurrentTime.Year,
                    month = photoSavedData.datePhoto.CurrentTime.Month,
                    day = photoSavedData.datePhoto.CurrentTime.Day,
                    hour = photoSavedData.datePhoto.CurrentTime.Hour,
                    minute = photoSavedData.datePhoto.CurrentTime.Minute
                };
                _photoApp.AddPhoto(photoData);
                _mapApp.SetPhotoHasBeenTaken(location.Data.locationName);
            }
        }
    }

    public void TakePhoto()
    {
        
        DateTime now = _phoneManager.ClockSystem.CurrentTimeData.CurrentTime;
        DateTime time = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        Sprite photo=_basePhoto;
        
        if (AppManager.Instance.GetApplication(ApplicationType.Map))
        {
            photo = _phoneManager.CurrentLocation.photo;
        }
        if (_phoneManager.CurrentLocation.hasPhotoBeenTaken) 
        {
            _phoneManager.CreateThought(" j'ai déjà pris cette photo");
            return;
        }
        Debug.Log("*Clic* New photo");
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
        _mapApp.SetPhotoHasBeenTaken(_phoneManager.CurrentLocation.locationName);
        SaveManager.instance.SaveLocationPhoto(_phoneManager.CurrentLocation.locationName, newPhoto, _phoneManager.Setup.Name);
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

