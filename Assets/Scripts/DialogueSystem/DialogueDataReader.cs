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

    public DialogueData dialogueData;

    private NodeData _currentNodeData;

    private MessageApp _messageApp;
    private ContactApp _contactApp;

    private string _characterID;
    private bool _isWaitingForInput = false;

    private EventTrigger.Entry _entry;
    private EventTrigger _eventTrigger;

    public string CharacterID { get => _characterID; set => _characterID = value; }

    private void OnEnable()
    {
        _messageApp = AppManager.Instance.GetApplication(ApplicationType.Messages) as MessageApp;
        _eventTrigger = gameObject.GetComponent<EventTrigger>();
        //Get dialogueData from contact app
        /*var contactApp = AppManager.Instance.GetApplication(ApplicationType.Contacts) as ContactApp;
        dialogueData = contactApp.*/
    }


    public void StartConversation()
    {
        _contactApp = AppManager.Instance.GetApplication(ApplicationType.Contacts) as ContactApp;
        List<NodeData> nodes = dialogueData.nodes;
        var affinityProperty = dialogueData.properties.FirstOrDefault(x => x.Name == "Affinity");
        //Get affinity from character ID
        //affinityProperty.SetValue()
        ReadNodeData(GetNextNodeData(dialogueData.nodes.FirstOrDefault(node => node.nodeGUID == dialogueData.entryPointNodeGuid))).Invoke();
        
    }

    private NodeData GetNextNodeData(NodeData currentNodeData, int outputID = 0)
    {
        if(currentNodeData == null) { return null; }
        return dialogueData.nodes.FirstOrDefault(node => node.nodeGUID == currentNodeData.outputs[outputID].targetNodeGuid);
    }

    private void ReadNextNode(NodeData currentNodeData, int outputID = 0)
    {
        currentNodeData.isSent = true;
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
                    if (dialogueNodeData.isSent)
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
                    
                    if (choiceData.isSent && choiceData.chosenChoiceID > -1)
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
                    ExposedProperty property = dialogueData.properties.FirstOrDefault(prop => prop.Name == setNodeData.property.Name);
                    if(property != null)
                    {
                        property.SetValue(ExposedProperty.GetValueFromString(property.type, setNodeData.valueString));
                    }
                    ReadNextNode(nodeData, 0);
                };

            default:
                return () => { };
        }
        
    }

    private void WaitForMouseClick()
    {
        _isWaitingForInput = true;
        _entry = new EventTrigger.Entry();
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
            if(!condition.Evaluate(dialogueData.properties)) return false;
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
}
