using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;


public class DialogueDataReader : MonoBehaviour
{
    
    public List<DialogueData> dialogueDatas = new List<DialogueData>();
    public bool isActive;
    public bool isBlocked;
    private bool _isWaitingForStart;
    private DialogueData _currentDialogueData;
    
    private NodeData _currentNodeData;
    private NodeData _lastNodeData;

    private MessageApp _messageApp;
    private ContactApp _contactApp;
    private NoteApp _noteApp;
    private HackApp _hackApp;

    private string _characterID;

    private EventTrigger.Entry _entry;
    private EventTrigger _eventTrigger;

    private GlobalPropertiesData _globalPropertiesData;

    private Sprite _askedImage = null;

    public event Action OnWrongPhotoSent;

    public string CharacterID { get => _characterID; set => _characterID = value; }
    public DialogueData CurrentDialogueData { get => _currentDialogueData; set => _currentDialogueData = value; }


    
    
    private void OnEnable()
    {
        _messageApp = AppManager.Instance.GetApplication(ApplicationType.Messages) as MessageApp;
        if (_messageApp == null) _messageApp = FindAnyObjectByType<MessageApp>();
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
        _hackApp = AppManager.Instance.GetApplication(ApplicationType.Hack) as HackApp;
        if (dialogueDatas.Count == 0) { return; }
        _currentDialogueData = SaveManager.Instance.LoadDialogue(conversationID, _messageApp.StoryName);
        _currentDialogueData.OnDialogueStatutChange += _messageApp.GetDiscussion(CharacterID).GetComponent<Discussion>().UpdateStatutConv;
        List<NodeData> nodes = _currentDialogueData.nodes;
        List<NodeData> temporaryNodes = _currentDialogueData.temporaryNodes;
        //_currentDialogueData = dialogueDatas.FirstOrDefault(data => data.name == conversationID);
        if (!_currentDialogueData.HasStarted && !_isWaitingForStart)
        {
            _isWaitingForStart = true;
            StartCoroutine(WaitForDialogueToStart());
            return;
        }

        //Check if temporaryNodes to display before starting?

        StartReadingDialogue();
        //var affinityProperty = _currentDialogueData.properties.FirstOrDefault(x => x.Name == "Affinity");

    }

    private void StartReadingDialogue()
    {
        CurrentDialogueData.HasStarted = true;
        ReadNodeData(GetNextNodeData(_currentDialogueData.nodes.FirstOrDefault(node => node.nodeGUID == _currentDialogueData.entryPointNodeGuid))).Invoke();
    }

    IEnumerator WaitForDialogueToStart()
    {
        yield return new WaitForSeconds(_currentDialogueData.GetSecondsToWait());
        Debug.Log("New conv");
        _isWaitingForStart = false;
        StartReadingDialogue();
        yield return null;
    }


    private NodeData GetNextNodeData(NodeData currentNodeData, int outputID = 0)
    {
        if(currentNodeData == null) { return null; }
        return _currentDialogueData.nodes.FirstOrDefault(node => node.nodeGUID == currentNodeData.outputs[outputID].targetNodeGuid);
    }

    private void ReadNextNode(NodeData currentNodeData, int outputID = 0, bool isChoice = false)
    {
        currentNodeData.isSentCurrent = true;
        
        var nextData = GetNextNodeData(currentNodeData, outputID);
        
        if(nextData == null) 
        { 
            Debug.Log("Fin de conv");  
            EndOfConversation();
            return;
        } 

        
        List<NodeData> tempNodes = _currentDialogueData.temporaryNodes.FindAll(node => node.outputs[0].targetNodeGuid == nextData.nodeGUID);
        foreach(var node in tempNodes)
        {
            ReadNodeData(node, false, true).Invoke();
        }

        SaveManager.Instance.SaveDialogue(_currentDialogueData, _messageApp.StoryName);
        _lastNodeData = _currentNodeData;
        ReadNodeData(nextData, isChoice).Invoke();
    }

    private void EndOfConversation()
    {
        _currentDialogueData.HasStarted = false;
    }

    private void ReadNextNode(string targetNodeGUID)
    {
        var nextNode = _currentDialogueData.nodes.Find(x => x.nodeGUID == targetNodeGUID);
        ReadNodeData(nextNode).Invoke();
    }


    public Action ReadNodeData(NodeData nodeData, bool isChoice = false, bool isTemporary = false)
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
                    if (dialogueNodeData.isSentCurrent || isChoice || isTemporary)
                    {
                        _messageApp.AddMessage(dialogueNodeData.dialogueText, dialogueNodeData.isNPC, _characterID, dialogueNodeData.emotion);

                        //Debug.Log("Message envoyé: " + dialogueNodeData.dialogueText);
                        if (!isTemporary)
                        {
                            ReadNextNode(nodeData, 0);
                        }
                    }
                    else
                    {
                        
                        if (dialogueNodeData.isNPC)
                        {
                            //Debug.Log("Message NPC: " + dialogueNodeData.dialogueText);
                            StartCoroutine(DelayMessage(dialogueNodeData, isTemporary));
                        }
                        else
                        {
                            //Debug.Log("Message joueur attente de clic: " + dialogueNodeData.dialogueText);
                            WaitForMouseClick(() => {
                                _messageApp.AddMessage(dialogueNodeData.dialogueText, dialogueNodeData.isNPC, _characterID);
                                if (!isTemporary)
                                {
                                    ReadNextNode(nodeData, 0);
                                }
                            });

                        }
                    }

                   
                };
            case NodeType.Choice:
                ChoiceNodeData choiceData = nodeData as ChoiceNodeData;
                return () =>
                {

                    /*if (!string.IsNullOrEmpty(choiceData.dialogueText))
                    {
                        _messageApp.AddMessage(choiceData.dialogueText, false, _characterID);
                    }

                    if(choiceData.isSentCurrent && choiceData.chosenChoiceID > -1)
                    {
                        ReadNextNode(nodeData, choiceData.chosenChoiceID);
                    }
                    else
                    {
                        //Change pour que le bouton le fasse
                        _messageApp.SendChoice(GetChoicesTexts(choiceData.outputs), _characterID);
                    }*/

                    /*if (choiceData.isSentCurrent && choiceData.chosenChoiceID > -1)
                    {
                        if (!string.IsNullOrEmpty(choiceData.dialogueText))
                        {
                            _messageApp.AddMessage(choiceData.dialogueText, false, _characterID);
                        }
                        ReadNextNode(nodeData, choiceData.chosenChoiceID);
                    }
                    else
                    {
                        WaitForMouseClick(() =>
                        {
                            _messageApp.SendChoice(GetChoicesTexts(choiceData.outputs), _characterID);
                        });
                    }*/
                    if (!string.IsNullOrEmpty(choiceData.dialogueText))
                    {
                        _messageApp.AddMessage(choiceData.dialogueText, false, _characterID);
                    }

                    if (choiceData.isSentCurrent && choiceData.chosenChoiceID > -1)
                    {

                        ReadNextNode(nodeData, choiceData.chosenChoiceID);
                    }
                    else
                    {
                        WaitForMouseClick(() =>
                        {
                            _messageApp.SendChoice(GetChoicesTexts(choiceData.outputs), _characterID);
                        });
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
            case NodeType.Block:
                BlockNodeData blockNodeData = nodeData as BlockNodeData;
                return () =>
                {
                    _currentDialogueData.IsLocked = true;
                };
            case NodeType.NewApplication:
                NewApplicationNodeData newAppNodeData = nodeData as NewApplicationNodeData;
                return () =>
                {
                    ApplicationType type = Enum.Parse<ApplicationType>(newAppNodeData.applicationType);
                    _messageApp.AddLinkTo(type, _characterID);
                    ReadNextNode(nodeData, 0);
                };
            case NodeType.NewFile:
                NewFileNodeData newFileNodeData = nodeData as NewFileNodeData;
                return () =>
                {
                    _hackApp.AddFolder(newFileNodeData.fileName);
                    ReadNextNode(nodeData, 0);
                };
            case NodeType.Time:
                TimeNodeData timeNodeData = nodeData as TimeNodeData;
                return () =>
                {
                    PhoneManager.Instance.ClockSystem.AddTime(timeNodeData.year, timeNodeData.month, timeNodeData.day, timeNodeData.hour, timeNodeData.minute);
                    ReadNextNode(nodeData, 0);
                };
            case NodeType.Image:
                ImageNodeData imageNodeData = nodeData as ImageNodeData;
                if (imageNodeData.isNPC || imageNodeData.isSentCurrent)
                {
                    print("UwU");
                }
                return () =>
                {
                    if (imageNodeData.isNPC || imageNodeData.isSentCurrent)
                    {
                        _messageApp.AddImage(imageNodeData.image, imageNodeData.isNPC, _characterID);
                        if (!isTemporary)
                        {
                            ReadNextNode(nodeData, 0);
                        }
                        
                    }
                    else
                    {
                        WaitForSendingImage(imageNodeData.image, _characterID);
                    }
                };
            default:
                return () => { };
        }
        
    }

    private void WaitForSendingImage(Sprite image, string ID)
    {
        _askedImage = image;
    }

    public void SendImage(Sprite image)
    {
        _messageApp.AddImage(image, false, _characterID);
    }

    public void SendImageFromPhotoApp(Sprite image)
    {
        OutputData outputData = new OutputData();
        outputData.portValue = "Next";
        if (_currentNodeData != null && _currentNodeData.nodeType != NodeType.Image) { _currentNodeData = _lastNodeData; }

        _messageApp.DisableSendingButton(_characterID);
        if(_currentNodeData != null)
        {

            if (_currentNodeData.nodeType == NodeType.Choice)
            {
                outputData.targetNodeGuid = GetNextNodeData(_currentNodeData, ((ChoiceNodeData)_currentNodeData).chosenChoiceID).nodeGUID;
            }
            else
            {
                outputData.targetNodeGuid = GetNextNodeData(_currentNodeData).nodeGUID;
            }
        }
        NodeData data = new NodeData();
        data.outputs.Add(outputData);
        data.nodeType = NodeType.Image;
        ImageNodeData newTemporaryData = new ImageNodeData(data);
        newTemporaryData.image = image;
        newTemporaryData.isNPC = false;
        newTemporaryData.isSentCurrent = true;
        if (_currentDialogueData != null) _currentDialogueData.temporaryNodes.Add(newTemporaryData);
        
        SendImage(image);
        SaveManager.Instance.SaveDialogue(_currentDialogueData, _messageApp.StoryName);
        CheckForImage(image, outputData.targetNodeGuid);
    }

    private void CheckForImage(Sprite sentImage, string targetGUID)
    {
        if (_askedImage != null && sentImage == _askedImage)
        {
            _askedImage = null;
            ReadNextNode(targetGUID); //A verif
        }
        else if (_askedImage == null || sentImage != _askedImage)
        {
            OnWrongPhotoSent.Invoke();
        }
    }

    public void CreateTemporaryDialogueNodeData(string text, CharaEmotion emotion, bool shouldContinue = false)
    {

        OutputData outputData = new OutputData();
        outputData.portValue = "Next";
        if (_currentNodeData.nodeType != NodeType.Image) { _currentNodeData = _lastNodeData; }
        _messageApp.DisableSendingButton(_characterID);
        if (_currentNodeData != null)
        {

            if (_currentNodeData.nodeType == NodeType.Choice)
            {
                outputData.targetNodeGuid = GetNextNodeData(_currentNodeData, ((ChoiceNodeData)_currentNodeData).chosenChoiceID).nodeGUID;
            }
            else
            {
                outputData.targetNodeGuid = GetNextNodeData(_currentNodeData).nodeGUID;
            }
        }
        NodeData data = new NodeData();
        data.outputs.Add(outputData);
        data.nodeType = NodeType.Dialogue;
        DialogueNodeData newTemporaryData = new DialogueNodeData(data);
        newTemporaryData.dialogueText = text;
        newTemporaryData.isNPC = true;
        newTemporaryData.isSentCurrent = true;
        newTemporaryData.emotion = emotion;
        if (_currentDialogueData != null) _currentDialogueData.temporaryNodes.Add(newTemporaryData);
        
        SaveManager.Instance.SaveDialogue(_currentDialogueData, _messageApp.StoryName);
        if (shouldContinue)
        {
            ReadNextNode(outputData.targetNodeGuid);
        }
    }

    private void WaitForMouseClick(Action sendingAction)
    {
        _messageApp.EnableSendingButton(sendingAction, _characterID);
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
            /*case NodeType.Choice:
                ChoiceNodeData choiceData = _currentNodeData as ChoiceNodeData;
                if (!string.IsNullOrEmpty(choiceData.dialogueText))
                {
                    _messageApp.AddMessage(choiceData.dialogueText, false, _characterID);
                }
                _messageApp.SendChoice(GetChoicesTexts(choiceData.outputs), _characterID);
                break;*/
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
        ReadNextNode(_currentNodeData, choiceID, true);

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

    IEnumerator DelayMessage(DialogueNodeData currentData, bool isTemporary = false)
    {
        //Effet de message en cours d'envoi 
        yield return new WaitForSeconds(currentData.timerSending);
        _messageApp.AddMessage(currentData.dialogueText, true, _characterID, currentData.emotion);
        if (!isTemporary)
        {
            ReadNextNode(currentData, 0);
        }
        

    }

    IEnumerator DelayImage(ImageNodeData currentData)
    {
        
        yield return new WaitForSeconds(currentData.timerSending);
        _messageApp.AddImage(currentData.image, true, _characterID);
        ReadNextNode(currentData, 0);

    }

    internal void UnlockDialogue(string dialogueID)
    {
        var data = dialogueDatas.FirstOrDefault(x => x.name == dialogueID);
        data.IsLocked = false;
        StartConversation(dialogueID);  
        Debug.Log($"Dialogue avec {CharacterID} est maintenant débloqué");
    }

    public bool IsChoicePossible(string choiceValue)
    {
        OutputData choice = GetChoiceFromText(choiceValue);
        NodeData nextNode = GetNextNodeData(_currentNodeData, _currentNodeData.outputs.IndexOf(choice));
        if (nextNode == null) return true;
        if(nextNode.nodeType == NodeType.Condition)
        {
            ConditionPropertyNodeData conditionNodeData = nextNode as ConditionPropertyNodeData;
            bool conditionValue = GetFinalConditionValue(conditionNodeData.conditions);
            return conditionValue;
        }
        return true;
    }

    
}
