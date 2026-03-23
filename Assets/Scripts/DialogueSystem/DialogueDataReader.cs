using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DialogueDataReader : MonoBehaviour
{

    public DialogueData dialogueData;

    private NodeData _currentNodeData;

    private MessageApp _messageApp;

    private string _characterID;

    public string CharacterID { get => _characterID; set => _characterID = value; }

    private void OnEnable()
    {
        _messageApp = AppManager.Instance.GetApplication(ApplicationType.Messages) as MessageApp;
    }


    public void StartConversation()
    {
        List<NodeData> nodes = dialogueData.nodes;
        ReadNodeData(GetNextNodeData(dialogueData.nodes.FirstOrDefault(node => node.nodeGUID == dialogueData.entryPointNodeGuid))).Invoke();
    }

    private NodeData GetNextNodeData(NodeData currentNodeData, int outputID = 0)
    {
        if(currentNodeData == null) { return null; }
        return dialogueData.nodes.FirstOrDefault(node => node.nodeGUID == currentNodeData.outputs[outputID].targetNodeGuid);
    }

    private void ReadNextNode(NodeData currentNodeData, int outputID = 0)
    {
        var nextData = GetNextNodeData(currentNodeData, outputID);
        if(nextData == null) { return; }
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
                    _messageApp.AddMessage(dialogueNodeData.dialogueText, dialogueNodeData.isNPC, _characterID);
                    ReadNextNode(nodeData, 0);
                };
            case NodeType.Choice:
                ChoiceNodeData choiceData = nodeData as ChoiceNodeData;
                return () =>
                {
                    if(!string.IsNullOrEmpty(choiceData.dialogueText))
                    {
                        _messageApp.AddMessage(choiceData.dialogueText, false, _characterID);
                    }
                    _messageApp.SendChoice(GetChoicesTexts(choiceData.outputs), _characterID);
                };
            default:
                return () => { };
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
        OutputData choice = GetChoiceFromText(choiceText);
        _messageApp.AddMessage(choiceText, false, _characterID);
        int choiceID = _currentNodeData.outputs.IndexOf(choice);
        ReadNextNode(_currentNodeData, choiceID);

    }

    IEnumerator Conversation()
    {
        
        yield return new WaitForSeconds(1f);
    }


}
