using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSheet", menuName = "Scriptable Objects/CharacterSheet")]
public class CharacterSheet : ScriptableObject
{
    [SerializeField] private string _CharacterName;
    [SerializeField] private SentText[] _baseText;
    [SerializeField] private string  _baseNotes;
    [SerializeField,Range(0,20)] private int  _baseAffinity;
    private Dictionary<CharaEmotion, Sprite> _profilePics;

    [SerializeField] private Sprite _messageBackground;
    [SerializeField] private DialogueData[] _dialogues;
    [SerializeField] private PhoneNumbers _telNum;
    [SerializeField] private List<String> _wrongPhotoResponses=new List<string>() ;
    public int dialogueIndex;

    public string Name { get => _CharacterName;
            set => _CharacterName = value; }

    public SentText[] BaseText { get => _baseText; }

    public List<EmotiionPic> EmotionPIcs=new List<EmotiionPic>();
    public string BaseNotes { get => _baseNotes; set => _baseNotes = value; }
    public int BaseAffinity { get => _baseAffinity; set=>_baseAffinity=value; }
    public PhoneNumbers TelNum { get => _telNum; set => _telNum = value; }
    public Dictionary<CharaEmotion, Sprite > ProfilePics { get
        {
            if(_profilePics != null)
            {
                return _profilePics;
            }
            _profilePics = new Dictionary<CharaEmotion,Sprite>();
            for(int i=0;i< EmotionPIcs.Count;i++)
            {
                _profilePics.Add(EmotionPIcs[i].Emotion, EmotionPIcs[i].Picture);
            }
            return _profilePics;
        }
    }
    public DialogueData currentDialogue
    {
        get
        {
            if (dialogueIndex >= _dialogues.Length) return null;
            return _dialogues[dialogueIndex];
        }
    }

    public DialogueData[] Dialogues { get => _dialogues; set => _dialogues = value; }
    public Sprite MessageBackground { get => _messageBackground;}
    public List<string> WrongPhotoResponses { get => _wrongPhotoResponses; }

    public Sprite GetBasePicture()
    {
        return _profilePics[CharaEmotion.Base];
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
    public CharaEmotion Emotion;
    public Sprite Picture;
}


