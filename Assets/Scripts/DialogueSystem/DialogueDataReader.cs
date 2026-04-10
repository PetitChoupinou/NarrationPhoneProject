using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;


public class DialogueDataReader : MonoBehaviour
{
    public List<DialogueData> dialogueDatas = new List<DialogueData>();
    private DialogueData _currentDialogueData;

    private NodeData _currentNodeData;

    private MessageApp _messageApp;
    private ContactApp _contactApp;
    private NoteApp _noteApp;

    private string _characterID;

    private EventTrigger.Entry _entry;
    private EventTrigger _eventTrigger;

    private GlobalPropertiesData _globalPropertiesData;

    public string CharacterID { get => _characterID; set => _characterID = value; }

    private void OnEnable()
    {
        _messageApp = AppManager.Instance.GetApplication(ApplicationType.Messages) as MessageApp;
        _eventTrigger = gameObject.GetComponent<EventTrigger>();
        _globalPropertiesData = Resources.Load<GlobalPropertiesData>("GlobalPropertiesData");

        //Get dialogueData from contact app
        /*var contactApp = AppManager.Instance.GetApplication(ApplicationType.Contacts) as ContactApp;
        dialogueData = contactApp.*/
    }


    public void StartConversation(string conversationID)
    {
        _contactApp = AppManager.Instance.GetApplication(ApplicationType.Contacts) as ContactApp;
        _noteApp = AppManager.Instance.GetApplication(ApplicationType.Notes) as NoteApp;
        if(dialogueDatas.Count == 0) { return; }
        _currentDialogueData = dialogueDatas.FirstOrDefault(data => data.name == conversationID);
        List<NodeData> nodes = _currentDialogueData.nodes;
        //var affinityProperty = _currentDialogueData.properties.FirstOrDefault(x => x.Name == "Affinity");
        ReadNodeData(GetNextNodeData(_currentDialogueData.nodes.FirstOrDefault(node => node.nodeGUID == _currentDialogueData.entryPointNodeGuid))).Invoke();
        
    }


    private NodeData GetNextNodeData(NodeData currentNodeData, int outputID = 0)
    {
        if(currentNodeData == null) { return null; }
        return _currentDialogueData.nodes.FirstOrDefault(node => node.nodeGUID == currentNodeData.outputs[outputID].targetNodeGuid);
    }

    private void ReadNextNode(NodeData currentNodeData, int outputID = 0)
    {
        currentNodeData.isSentCurrent = true;
        var nextData = GetNextNodeData(currentNodeData, outputID);
        if(nextData == null) { return; } // End of conversation
        ReadNodeData(nextData).Invoke();
    }

    public Action ReadNodeData(NodeData nodeData)
    {
        _currentNodeData = nodeData;
        
        if (nodeData == null)
            return () => { };
        switch (nodeData.nodeType)
        {

            case NodeType.Dialogue:
                DialogueNodeData dialogueNodeData = nodeData as DialogueNodeData;
                return () =>
                {
                    if (dialogueNodeData.isSentCurrent)
                    {
                        _messageApp.AddMessage(dialogueNodeData.dialogueText, dialogueNodeData.isNPC, _characterID);
                        ReadNextNode(nodeData, 0);
                    }
                    else
                    {
                        
                        if (dialogueNodeData.isNPC)
                        {
                            StartCoroutine(DelayMessage(dialogueNodeData));
                        }
                        else
                        {
                            WaitForMouseClick();
                        }
                    }

                   
                };
            case NodeType.Choice:
                ChoiceNodeData choiceData = nodeData as ChoiceNodeData;
                return () =>
                {
                    
                    if (choiceData.isSentCurrent && choiceData.chosenChoiceID > -1)
                    {
                        if (!string.IsNullOrEmpty(choiceData.dialogueText))
                        {
                            _messageApp.AddMessage(choiceData.dialogueText, false, _characterID);
                        }
                        ReadNextNode(nodeData, choiceData.chosenChoiceID);
                    }
                    else
                    {
                        WaitForMouseClick();
                    }
                    
                    
                };
            case NodeType.Affinity:
                AffinityNodeData affinityNodeData = nodeData as AffinityNodeData;
                return () =>
                {
                    
                    _contactApp.GainRelation(affinityNodeData.affinityGain, _characterID);
                    ReadNextNode(nodeData, 0);
                };
            case NodeType.Condition:
                ConditionPropertyNodeData conditionNodeData = nodeData as ConditionPropertyNodeData;
                return () =>
                {
                    
                    if (GetFinalConditionValue(conditionNodeData.conditions))
                    {
                        ReadNextNode(nodeData, 0);
                    }
                    else
                    {
                        ReadNextNode(nodeData, 1);
                    }

                };

            case NodeType.Set:
                SetPropertyNodeData setNodeData = nodeData as SetPropertyNodeData;
                return () =>
                {
                    ExposedProperty property = _globalPropertiesData.globalProperties.FirstOrDefault(prop => prop.Name == setNodeData.property.Name);
                    if (property != null)
                    {
                        property.SetValue(ExposedProperty.GetValueFromString(property.type, setNodeData.valueString));
                    }
                    ReadNextNode(nodeData, 0);
                };
            case NodeType.Unlock:
                UnlockNodeData unlockNodeData = nodeData as UnlockNodeData;
                Debug.Log(_currentNodeData);
                return () =>
                {
                    _messageApp.UnlockDialogue(unlockNodeData.characterID, unlockNodeData.dialogueID);
                    ReadNextNode(nodeData, 0);
                };
            case NodeType.Thinking:
                ThinkingNodeData thinkingNodeData = nodeData as ThinkingNodeData;
                return () =>
                {
                    _messageApp.CreateThought(thinkingNodeData.text, _characterID);
                    ReadNextNode(nodeData, 0);
                };
            case NodeType.Note:
                NoteNodeData noteNodeData = nodeData as NoteNodeData;
                return () =>
                {
                    foreach(NoteData note in noteNodeData.notesData)
                    {
                        _noteApp.AddNote(note.data.title, note.data.content);
                    }
                    
                    ReadNextNode(nodeData, 0);
                };
            default:
                return () => { };
        }
        
    }

    private void WaitForMouseClick()
    {
        _entry = new Entry();
        _entry.eventID = EventTriggerType.PointerClick;
        _entry.callback.AddListener(OnClick);

        _eventTrigger.triggers.Add(_entry);
    }

    private void OnClick(BaseEventData data)
    {
        switch (_currentNodeData.nodeType)
        {
            case NodeType.Dialogue:
                DialogueNodeData dialogueNodeData = _currentNodeData as DialogueNodeData;
                _messageApp.AddMessage(dialogueNodeData.dialogueText, dialogueNodeData.isNPC, _characterID);
                ReadNextNode(dialogueNodeData, 0);
                break;
            case NodeType.Choice:
                ChoiceNodeData choiceData = _currentNodeData as ChoiceNodeData;
                if (!string.IsNullOrEmpty(choiceData.dialogueText))
                {
                    _messageApp.AddMessage(choiceData.dialogueText, false, _characterID);
                }
                _messageApp.SendChoice(GetChoicesTexts(choiceData.outputs), _characterID);
                break;
        }

        if (_eventTrigger != null && _entry != null)
        {
            _eventTrigger.triggers.Remove(_entry);
        }
    }

    List<string> GetChoicesTexts(List<OutputData> choices)
    {
        List<string> choicesTexts = new List<string>();
        for (int i = 0; i < choices.Count; i++)
        {
            choicesTexts.Add(choices[i].portValue);
        }

        return choicesTexts;
    }

    public OutputData GetChoiceFromText(string text)
    {
        OutputData choice = _currentNodeData.outputs.FirstOrDefault(output => output.portValue == text);
        return choice;
    }

    public void MakeChoice(string choiceText)
    {
        ChoiceNodeData choiceNodeData = _currentNodeData as ChoiceNodeData;
        OutputData choice = GetChoiceFromText(choiceText);
        int choiceID = _currentNodeData.outputs.IndexOf(choice);
        choiceNodeData.chosenChoiceID = choiceID;
        ReadNextNode(_currentNodeData, choiceID);

    }

    public bool GetFinalConditionValue(List<Condition> conditions)
    {

        foreach (var condition in conditions)
        {
            if (!condition.Evaluate(_globalPropertiesData.globalProperties)) return false;
        }
        return true;
    }


    IEnumerator Conversation()
    {
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator DelayMessage(DialogueNodeData currentData)
    {
        //Effet de message en cours d'envoi 
        yield return new WaitForSeconds(currentData.timerSending);
        _messageApp.AddMessage(currentData.dialogueText, true, _characterID);
        ReadNextNode(currentData, 0);

    }

    internal void UnlockDialogue(string dialogueID)
    {
        var data = dialogueDatas.FirstOrDefault(x => x.name == dialogueID);
        data.isLocked = false;
        StartConversation(dialogueID);  
        Debug.Log($"Dialogue avec {CharacterID} est maintenant débloqué");
    }
}
