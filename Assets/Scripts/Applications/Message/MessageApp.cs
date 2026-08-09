using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore.Text;
using System.Linq;
using System;
using UnityEngine.UI;

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

    public GameObject CurrentConv { get => _currentConv;}
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
            discussion.GetComponent<Discussion>().SetUp(name, texts, button, _headerText ,background);

            gameObjectsToDeactivate.Add(discussion);
            _discussions.Add(discussion.GetComponent<Discussion>());
            DialogueDataReader dialogueDataReader = discussion.GetComponent<DialogueDataReader>();
            //dialogueDataReader._currentDialogueData = character.currentDialogue;
            dialogueDataReader.dialogueDatas.AddRange(character.Dialogues);
        }

        StartCoroutine(StartGame());
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
    public void AddMessage(string text, bool isNPC,string ID)
    {
        var discussion = _discussions.Find(x => x.ID == ID);
        
        discussion.AddMessage(text, isNPC);
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
                var availableData = dialogueDataReader.dialogueDatas.FirstOrDefault(x => x.isLocked == false);
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
