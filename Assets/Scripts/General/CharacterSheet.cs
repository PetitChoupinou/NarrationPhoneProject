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
    [SerializeField] private List<DialogueData> _dialogues=new List<DialogueData>();
    [SerializeField] private PhoneNumbers _telNum;
    [SerializeField] private List<String> _wrongPhotoResponses=new List<string>() ;
    public int dialogueIndex;

    public string Name { get => _CharacterName;
            set => _CharacterName = value; }

    public SentText[] BaseText { get => _baseText; }

    public List<EmotionPic> EmotionPIcs=new List<EmotionPic>();
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
            if (dialogueIndex >= _dialogues.Count) return null;
            return _dialogues[dialogueIndex];
        }
    }

    public List<DialogueData> Dialogues { get => _dialogues; set => _dialogues = value; }
    public Sprite MessageBackground { get => _messageBackground; set => _messageBackground = value; }
    public List<string> WrongPhotoResponses { get => _wrongPhotoResponses; }

    public Sprite GetBasePicture()
    {
        return _profilePics[CharaEmotion.Base];
    }
    public CharacterSheet()
    {
        _telNum.callText = new List<string>();
    }
    public void AddDialogue()
    {
        _dialogues.Add(new DialogueData());
    }
    public void RemoveDialogue(int appPos)
    {
        _dialogues.RemoveAt(appPos);
    }
    public void SetDialogue(int i, DialogueData obj)
    {
        _dialogues[i] = obj;
    }
    public void AddWPR()
    {
        _wrongPhotoResponses.Add(null);
    }
    public void RemoveWPR(int appPos)
    {
        _wrongPhotoResponses.RemoveAt(appPos);
    }
    public void SetWPR(int i, string obj)
    {
        _wrongPhotoResponses[i] = obj;
    }
}

[Serializable]
public class SentText
{
    public string Text;
    public bool isNPC; 
}
[Serializable]
public class EmotionPic
{
    public CharaEmotion Emotion;
    public Sprite Picture;
}


