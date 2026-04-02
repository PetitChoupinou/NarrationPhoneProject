using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryAppSetup", menuName = "Scriptable Objects/StoryAppSetup")]
public class StoryAppSetup : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] List<CharacterSheet> _characters = new List<CharacterSheet>();
    [SerializeField] List<PhotoData> _photos = new List<PhotoData>();
    [SerializeField] List<NotesData> _notes = new List<NotesData>();


    public string Name { get => _name;}
    public List<CharacterSheet> Characters { get => _characters;}
    public List<PhotoData> Photos { get => _photos;}
    public List<NotesData> Notes { get => _notes;}
}

[Serializable]
public struct NotesData
{
    public string title;
    public string content;
}
