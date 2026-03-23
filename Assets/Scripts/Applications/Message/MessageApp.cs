using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class MessageApp : Application
{
    private List<GameObject> gameObjectsToDeactivate=new List<GameObject>();
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private GameObject _discussionPrefab;
    [SerializeField] private GameObject _buttonCanvas;
    [SerializeField] private GameObject _headerButton;
    [SerializeField] private TMP_Text _headerText;
    private List<Discussion> _discussions = new List<Discussion>();
    private GameObject _currentConv;

    public GameObject CurrentConv { get => _currentConv; set => _currentConv = value; }

    override public  void SetUp(List<CharacterSheet> characters)
    {
        // Debug => only character 0 (Sasha) displayed here => NEED CHANGES

        for (int i = 0; i < characters.Count; i++)
        {
            CharacterSheet character = characters[i];
            string name = character.Name;
            SentText[] texts = character.BaseText;
            GameObject button = Instantiate(_buttonPrefab, _buttonCanvas.transform);
            GameObject discussion = Instantiate(_discussionPrefab, transform);

            button.GetComponent<InAppButton>().SetUp(name, discussion, _headerButton);
            discussion.GetComponent<Discussion>().SetUp(name, texts, button, _headerText);

            gameObjectsToDeactivate.Add(discussion);
            _discussions.Add(discussion.GetComponent<Discussion>());
            DialogueDataReader dialogueDataReader = discussion.GetComponent<DialogueDataReader>();
            if (dialogueDataReader != null)
            {
                dialogueDataReader.dialogueData = character.currentDialogue;
                if(dialogueDataReader.dialogueData != null) dialogueDataReader.StartConversation();

            }
        }

        StartCoroutine(StartGame());
    }
    public override void CloseCurrent()
    {
        if (CurrentConv == null) return;
        _currentConv.SetActive(false);
        _headerText.text = "message";
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.app);
        _buttonCanvas.SetActive(true);
        _headerButton.SetActive(false);
        _currentConv = null;
    }
    public void AddMessage(string text, bool isNPC,string ID)
    {
        var discussion = _discussions.Find(x => x.ID == ID);
        //Debug.Log(_discussions.Count + " " + this.name);
        discussion.AddMessage(text, isNPC);
    }
    public void SendChoice(List<string> choices,string ID)
    {
        _discussions.Find(x => x.ID == ID).TriggerChoice(choices);


    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(.02f);
        for (int i = 0; i < gameObjectsToDeactivate.Count; i++)
        {
            gameObjectsToDeactivate[i].SetActive(false);
        }
        yield return null;
    }

    public void GainAffinity(float value, string targetID)
    {
        //Get the character with the targetID and increase their affinity by value
        Debug.Log($"You gain {value} affinity with {targetID}!");
    }

}
