using System;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSheet", menuName = "Scriptable Objects/CharacterSheet")]
public class CharacterSheet : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private SentText[] baseText;
    [SerializeField] private string  baseNotes;
    [SerializeField,Range(0,20)] private int  baseAffinity;
    private Dictionary<string, Sprite> profilePics;

    [SerializeField] private Sprite messageBackground;
    [SerializeField] private DialogueData[] dialogues;
    [SerializeField] private PhoneNumbers telNum;
    public int dialogueIndex;

    public string Name { get => name;}
    public SentText[] BaseText { get => baseText; }

    public List<EmotiionPic> EmotionPIcs=new List<EmotiionPic>();
    public string BaseNotes { get => baseNotes;}
    public int BaseAffinity { get => baseAffinity;}
    public PhoneNumbers TelNum { get => telNum; }
    public Dictionary<string,Sprite > ProfilePics { get
        {
            if(profilePics != null)
            {
                return profilePics;
            }
            profilePics = new Dictionary<string,Sprite>();
            for(int i=0;i< EmotionPIcs.Count;i++)
            {
                profilePics.Add(EmotionPIcs[i].Emotion, EmotionPIcs[i].Picture);
            }
            return profilePics;
        }
    }
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

    public Sprite GetBasePicture()
    {
        foreach (string s in ProfilePics.Keys)
        {
            Debug.Log(s);
        }
        return profilePics["Base"];
    }
}

[Serializable]
public class SentText
{
    public string Text;
    public bool isNPC; 
}
[Serializable]
public class EmotiionPic
{
    public string Emotion;
    public Sprite Picture;
}


