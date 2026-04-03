using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryAppSetup", menuName = "Scriptable Objects/StoryAppSetup")]
public class StoryAppSetup : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] List<GameObject> _applications = new List<GameObject>();
    [SerializeField] List<CharacterSheet> _characters = new List<CharacterSheet>();
    [SerializeField] List<PhotoData> _photos = new List<PhotoData>();
    [SerializeField] List<NotesData> _notes = new List<NotesData>();
    [SerializeField] List<ClocksData> _clocks = new List<ClocksData>();
    [SerializeField] List<AlarmsData> _alarms = new List<AlarmsData>();


    public string Name { get => _name;}
    public List<GameObject> Applications { get => _applications; }

    public List<CharacterSheet> Characters { get => _characters;}
    public List<PhotoData> Photos { get => _photos;}
    public List<NotesData> Notes { get => _notes;}
    public List<ClocksData> Clocks { get => _clocks; }
    public List<AlarmsData> Alarms { get => _alarms;}
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
