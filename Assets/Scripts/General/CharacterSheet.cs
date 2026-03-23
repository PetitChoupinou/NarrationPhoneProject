using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSheet", menuName = "Scriptable Objects/CharacterSheet")]
public class CharacterSheet : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private SentText[] baseText;
    [SerializeField] private string  baseNotes;
    [SerializeField] private DialogueData[] dialogues;
    private float affinity;
    public int dialogueIndex;

    public string Name { get => name;}
    public SentText[] BaseText { get => baseText; }
    public string BaseNotes { get => baseNotes;}
    public DialogueData currentDialogue
    {
        get
        {
            if (dialogueIndex >= dialogues.Length) return null;
            return dialogues[dialogueIndex];
        }
    }

    public float Affinity { get => affinity; set => affinity = value; }
}
[Serializable]
public class SentText
{
    public string Text;
    public bool isNPC; 
}

