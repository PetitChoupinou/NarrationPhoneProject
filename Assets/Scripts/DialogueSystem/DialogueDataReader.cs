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
                    //SendNewMessage(dialogueNodeData.dialogueText);
                    Debug.Log("AddMessage");
                    _messageApp.AddMessage(dialogueNodeData.dialogueText, true, dialogueData.characterID);
                    ReadNextNode(nodeData, 0);
                };
            case NodeType.Choice:
                ChoiceNodeData choiceData = nodeData as ChoiceNodeData;
                return () =>
                {
                    //DisplayChoices(choiceData.outputs);
                    if(string.IsNullOrEmpty(choiceData.dialogueText))
                    {
                        _messageApp.AddMessage(choiceData.dialogueText, false, dialogueData.characterID);
                    }
                    _messageApp.SendChoice(GetChoicesTexts(choiceData.outputs), dialogueData.characterID);
                };
            default:
                return () => { };
        }
        
    }
    //Faire quoi ca choisisses bien le bon choice et hop
    void SendNewMessage(string text)
    {
        //Debug.Log(text);
        
    }

    void DisplayChoices(List<OutputData> choices)
    {
        //=> More like display one choice to test
        int id = UnityEngine.Random.Range(0, choices.Count);
        var choice = choices[id];
        //Debug.Log("Choix " + id + ": " + choices[id].portValue);
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
        _messageApp.AddMessage(choiceText, false, dialogueData.characterID);
        int choiceID = _currentNodeData.outputs.IndexOf(choice);
        ReadNextNode(_currentNodeData, choiceID);

    }

    IEnumerator Conversation()
    {
        
        yield return new WaitForSeconds(1f);
    }


}
