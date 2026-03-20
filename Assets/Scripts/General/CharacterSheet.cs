using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSheet", menuName = "Scriptable Objects/CharacterSheet")]
public class CharacterSheet : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private SentText[] baseText;
    [SerializeField] private string  baseNotes;

    public string Name { get => name;}
    public SentText[] BaseText { get => baseText; }
    public string BaseNotes { get => baseNotes;}
}
[Serializable]
public class SentText
{
    public string Text;
    public bool isNPC; 
}
