using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSheet", menuName = "Scriptable Objects/CharacterSheet")]
public class CharacterSheet : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private SentText[] baseText;
    [SerializeField] private string  baseNotes;
    [SerializeField,Range(-10,10)] private int  baseAffinity;
    [SerializeField,Header("+33")] private string telNum;
    [SerializeField] private Sprite profilePic;
    [SerializeField] private DialogueData[] dialogues;
    public int dialogueIndex;

    public string Name { get => name;}
    public SentText[] BaseText { get => baseText; }
    public string BaseNotes { get => baseNotes;}
    public int BaseAffinity { get => baseAffinity;}
    public string TelNum { get => telNum; }
    public Sprite ProfilePic { get => profilePic;}
    public DialogueData currentDialogue
    {
        get
        {
            if (dialogueIndex >= dialogues.Length) return null;
            return dialogues[dialogueIndex];
        }
    }
}
[Serializable]
public class SentText
{
    public string Text;
    public bool isNPC; 
}

