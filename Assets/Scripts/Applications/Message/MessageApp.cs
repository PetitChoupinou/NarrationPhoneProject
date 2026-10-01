using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.Audio.GeneratorInstance;

public class MessageApp : BaseApplication
{
    private List<GameObject> gameObjectsToDeactivate=new List<GameObject>();
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private GameObject _discussionPrefab;
    [SerializeField] private GameObject _buttonCanvas;
    [SerializeField] private GameObject _headerButton;
    [SerializeField] private TMP_Text _headerText;
    private List<Discussion> _discussions = new List<Discussion>();
    [SerializeField] private GameObject _currentConv;
    [SerializeField] private Image _bgImage;
    private Sprite _baseBackground;
    private string _storyName;
    private StoryAppSetup _setup;

    public GameObject CurrentConv { get => _currentConv;}
    public string StoryName { get => _storyName;}

    public void SetBackground(Sprite background)
    {
        if (_bgImage == null)
        {
            return;
        }
        _bgImage.sprite = background;
    }
    public void SetCurrentConv(GameObject conv)
    {
        _currentConv = conv;
    }
    public override void SetUp(StoryAppSetup setup)
    {
        _setup = setup;
        _storyName = setup.Name;
        List<CharacterSheet> characters = setup.Characters;
        _baseBackground= setup.MessageBackGround;
        SetBackground(_baseBackground);
        for (int i = 0; i < characters.Count; i++)
        {
            CharacterSheet character = characters[i];
            string name = character.Name;
            SentText[] texts = character.BaseText;
            GameObject button = Instantiate(_buttonPrefab, _buttonCanvas.transform);
            GameObject discussion = Instantiate(_discussionPrefab, transform);
            Sprite background = character.MessageBackground;
            discussion.name = "message " + name;
            button.GetComponent<InAppButton>().SetUp(name, discussion, _headerButton);
            discussion.GetComponent<Discussion>().SetUp(name, texts, button, _headerText, background, character.ProfilePics, character.WrongPhotoResponses);

            gameObjectsToDeactivate.Add(discussion);
            _discussions.Add(discussion.GetComponent<Discussion>());
            DialogueDataReader dialogueDataReader = discussion.GetComponent<DialogueDataReader>();
            
            //dialogueDataReader._currentDialogueData = character.currentDialogue;
            dialogueDataReader.dialogueDatas.AddRange(character.Dialogues);
        }
        // Get la _save => mettre les dialogues de la _save dans le data reader
        var save = SaveManager.Instance.Save;
        print(SaveManager.Instance.Save.name);
        StartCoroutine(StartGame());
    }
    public void AddEndConversation()
    {
        string name = _setup.EndGameContact.phone.title;
        SentText[] texts = null;
        Sprite background = _baseBackground;
        GameObject button = Instantiate(_buttonPrefab, _buttonCanvas.transform);
        GameObject discussion = Instantiate(_discussionPrefab, transform);
        discussion.name = "message " + name;
        Dictionary<CharaEmotion, Sprite> endProfilePics = new Dictionary<CharaEmotion, Sprite>();
        endProfilePics.Add(CharaEmotion.Base, _setup.EndGameContact.profilePic);
        button.GetComponent<InAppButton>().SetUp(name, discussion, _headerButton);
        discussion.GetComponent<Discussion>().SetUp(name, texts, button, _headerText, background, endProfilePics,null, _setup.EndGameContact.shouldShowPictureInConversation);
        DialogueDataReader dialogueDataReader = discussion.GetComponent<DialogueDataReader>();
        _discussions.Add(discussion.GetComponent<Discussion>());
        dialogueDataReader.dialogueDatas.AddRange(_setup.EndGameContact.dialogues);
        dialogueDataReader.StartConversation(name);
        Destroy(_headerButton);
        PhoneManager.Instance.DeactivatePhoneButtons();
    }
    public override void CloseCurrent()
    {
        if (!GetComponent<Canvas>().isActiveAndEnabled) return;
        _currentConv = GetCurrentDiscussion().gameObject;
        if (_currentConv == null) 
        {
            return;
        }
        if (_baseBackground == null)
        {
            SetBackground(null);
        }
        else 
        {
            SetBackground(_baseBackground);
        }    
        _currentConv.GetComponent<RectTransform>().localScale=Vector3.zero;
        _headerText.text = "message";
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.app);
        _buttonCanvas.SetActive(true);
        _headerButton.SetActive(false);
        _currentConv.GetComponent<Discussion>().IsEnabled = false;
        _currentConv = null;
    }
    public void AddMessage(string text, bool isNPC,string ID, CharaEmotion emotion = CharaEmotion.Base)
    {
        var discussion = _discussions.Find(x => x.ID == ID);
        
        discussion.AddMessage(text, isNPC, emotion);
    }

    public void AddImage(Sprite image, bool isNPC, string ID)
    {
        var discussion = _discussions.Find(x => x.ID == ID);

        discussion.AddImage(image, isNPC);
    }

    public void AddLinkTo(ApplicationType applicationType, string ID)
    {
        var discussion = _discussions.Find(x => x.ID == ID);
        discussion.AddLinkTo(applicationType);
    }
    public void SendChoice(List<string> choices,string ID)
    {
        _discussions.Find(x => x.ID == ID).TriggerChoice(choices);


    }
    public void CreateThought(string thought, string ID)
    {
        _discussions.Find(x => x.ID == ID).CreateThought(thought);
    }

    public void EnableSendingButton(Action sendingAction, string ID)
    {
        var discussion = _discussions.Find(x => x.ID == ID);
        discussion.EnableSendingButton(sendingAction);
    }

    public void DisableSendingButton(string ID)
    {
        var discussion = _discussions.Find(x => x.ID == ID);
        discussion.DisableSendingButton();
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(.02f);
        for (int i = 0; i < gameObjectsToDeactivate.Count; i++)
        {
            gameObjectsToDeactivate[i].GetComponent<RectTransform>().localScale=Vector3.zero;
        }
        foreach(var discussion in _discussions)
        {
            DialogueDataReader dialogueDataReader = discussion.GetComponent<DialogueDataReader>();
            if (dialogueDataReader != null && dialogueDataReader.dialogueDatas.Count > 0)
            {
                var availableData = dialogueDataReader.dialogueDatas.FirstOrDefault(x => x.IsLocked == false);
                if(availableData != null) dialogueDataReader.StartConversation(availableData.name);
            }
        }
        yield return null;
    }


    void StartConversation(string characterID)
    {
        _discussions.Find(x => x.ID == characterID).Enable();
    }

    public void UnlockDialogue(string characterID, string dialogueID)
    {
        var foundDialogue = _discussions.Find(x => x.ID == characterID);
        if(foundDialogue != null) foundDialogue.DialogueDataReader.UnlockDialogue(dialogueID);
        else Debug.LogError($"No dialogue '{dialogueID}' found for character ID: {characterID}");
    }
    public void NetworkIsGood()
    {
        foreach(Discussion d in _discussions)
        {
            d.DequeuPendingMessages();
        }
    }

    public Discussion  GetDiscussion(string ID)
    {
        return _discussions.Find(x => x.ID == ID);
    }
    public Discussion GetCurrentDiscussion()
    {
        return _discussions.Find(x => x.IsEnabled==true);
    }
}
