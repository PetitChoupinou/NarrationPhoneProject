using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSheet", menuName = "Scriptable Objects/CharacterSheet")]
public class CharacterSheet : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private SentText[] baseText;
    [SerializeField] private string  baseNotes;
    [SerializeField,Range(-10,10)] private int  baseLikeness;
    [SerializeField,Header("+33")] private string telNum;
    [SerializeField] private Sprite profilePic;

    public string Name { get => name;}
    public SentText[] BaseText { get => baseText; }
    public string BaseNotes { get => baseNotes;}
    public int BaseLikeness { get => baseLikeness;}
    public string TelNum { get => telNum; }
    public Sprite ProfilePic { get => profilePic;}
}
[Serializable]
public class SentText
{
    public string Text;
    public bool isNPC; 
}
