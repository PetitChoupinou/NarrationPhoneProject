using System;
using UnityEngine;

public class CameraApp : Application
{
    public override void CloseCurrent()
    {
        
    }

    public override void SetUp(StoryAppSetup setup)
    {
        
    }

    public void TakePhoto()
    {
        Debug.Log("*Clic* New photo");
        PhotoApp photoApp = (PhotoApp)AppManager.Instance.GetApplication(ApplicationType.Photos);
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
        photoApp.AddPhoto(newPhoto);
    }
}

