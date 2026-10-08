using System;
using UnityEngine;

[Serializable]
public class LocationPhotoData
{
    public string locationName;
    public TimeData datePhoto;
    public Sprite photo;

    public LocationPhotoData(DateTime time, Sprite photo)
    {
        datePhoto = new TimeData();
        datePhoto.CurrentTime = time;
        datePhoto.SetTimeFromCurrentTime();
        this.photo = photo;
    }
}
[Serializable]
public class BasePhotoData
{
    public TimeData datePhoto;
    public Sprite photo;

    public BasePhotoData(DateTime time, Sprite photo)
    {
        datePhoto = new TimeData();
        datePhoto.CurrentTime = time;
        datePhoto.SetTimeFromCurrentTime();
        this.photo = photo;
    }
}
