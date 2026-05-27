using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraApp : BaseApplication
{
    [SerializeField] private Image _thumbnail;
    private PhotoApp _photoApp;
    public override void CloseCurrent()
    {
        
    }

    public override void SetUp(StoryAppSetup setup)
    {
        
    }

    public void TakePhoto()
    {
        Debug.Log("*Clic* New photo");
        DateTime now = DateTime.Now;
        DateTime time = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        PhotoData newPhoto = new PhotoData
        {
            image = PhoneManager.Instance.CurrentLocation.photo,
            year = time.Year,
            month = time.Month,
            day = time.Day,
            hour = time.Hour,
            minute = time.Minute
        };
        _photoApp.AddPhoto(newPhoto);
        _thumbnail.sprite = newPhoto.image;
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
        _photoApp = (PhotoApp)AppManager.Instance.GetApplication(ApplicationType.Photos);
        _thumbnail.sprite = _photoApp.GetLatestPhoto().sprite;
    }
}

