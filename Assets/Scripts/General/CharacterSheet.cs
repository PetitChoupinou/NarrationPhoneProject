using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSheet", menuName = "Scriptable Objects/CharacterSheet")]
public class CharacterSheet : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private SentText[] baseText;
    [SerializeField] private string  baseNotes;
    [SerializeField,Range(0,20)] private int  baseAffinity;
    [SerializeField] private Sprite profilePic;
    [SerializeField] private Sprite messageBackground;
    [SerializeField] private DialogueData[] dialogues;
    [SerializeField] private PhoneNumbers telNum;
    public int dialogueIndex;

    public string Name { get => name;}
    public SentText[] BaseText { get => baseText; }
    public string BaseNotes { get => baseNotes;}
    public int BaseAffinity { get => baseAffinity;}
    public PhoneNumbers TelNum { get => telNum; }
    public Sprite ProfilePic { get => profilePic;}
    public DialogueData currentDialogue
    {
        get
        {
            if (dialogueIndex >= dialogues.Length) return null;
            return dialogues[dialogueIndex];
        }
    }

    public DialogueData[] Dialogues { get => dialogues; set => dialogues = value; }
    public Sprite MessageBackground { get => messageBackground;}
}

[Serializable]
public class SentText
{
    public string Text;
    public bool isNPC; 
}


