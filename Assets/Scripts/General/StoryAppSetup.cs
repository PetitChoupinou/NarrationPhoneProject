using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryAppSetup", menuName = "Scriptable Objects/StoryAppSetup")]
public class StoryAppSetup : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] List<GameObject> _applications = new List<GameObject>();
    [SerializeField] List<CharacterSheet> _characters = new List<CharacterSheet>();
    [SerializeField] List<PhotoPreviews> _photos = new List<PhotoPreviews>();
    [SerializeField] List<NotesData> _notes = new List<NotesData>();
    [SerializeField] List<ClocksData> _clocks = new List<ClocksData>();
    [SerializeField] List<AlarmsData> _alarms = new List<AlarmsData>();
    [SerializeField] List<PhoneNumbers> _phoneNumbers = new List<PhoneNumbers>();
    [SerializeField] List<InternetSerach> _internetSearches = new List<InternetSerach>();
    List<LocationData> _locations = new List<LocationData>();


    public string Name { get => _name;}
    public List<GameObject> Applications { get => _applications; }

    public List<CharacterSheet> Characters { get => _characters;}
    public List<PhotoPreviews> Photos { get => _photos;}
    public List<NotesData> Notes { get => _notes;}
    public List<ClocksData> Clocks { get => _clocks; }
    public List<AlarmsData> Alarms { get => _alarms;}
    public List<PhoneNumbers> PhoneNumbers { get => _phoneNumbers;}

    public List<InternetSerach> InternetSeraches { get => _internetSearches; }
    public List<LocationData> Locations { get => _locations; }

}

[Serializable]
public struct NotesData
{
    public string title;
    public string content;
}
[Serializable]
public struct AlarmsData
{
    public string tag;
    public AlarmRepetition repetition;
    public bool isActive;
    [Range(0,23)]public int hours;
    [Range(0,59)]public int minutes;
}
[Serializable]
public struct ClocksData
{
    public string Town;
    public int timeDiff;
}
[Serializable]
public struct PhotoData
{
    public Sprite image;
    public int year;
    [Range(1, 12)] public int month;
    [Range(1, 31)] public int day;
    [Range(0, 23)] public int hour;
    [Range(0, 59)] public int minute;
}

[Serializable]
public struct PhotoPreviews
{
   public string title;
   public bool locked;
   public string password;
    public List<PhotoData> photoDatas;
}

[Serializable]
public struct LocationData
{
    public string locationName;
    public Sprite image;
    public Vector2 coordinates;
    public Sprite photo;
    public NetworkState networkState;
}


[Serializable]
public struct PhoneNumbers
{
    public string title;
    public string numbers;
    //whatever it is supposed to be
}

[Serializable]
public struct InternetSerach
{
    public string search;
    public string text;
}
public enum NetworkState
{
    Bad,
    Mid,
    Good
};