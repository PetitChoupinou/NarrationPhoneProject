using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DialogueDataReader : MonoBehaviour
{
    //TODO: Read the dialogue data from the file and return a list of actions (SendMessage, WaitSomeTime, etc.)

    public DialogueData dialogueData;

    private NodeData _currentNodeData;

    private void Start()
    {
        List<NodeData> nodes = dialogueData.nodes;
        //Start the conversation => Need a coroutine
        ReadNodeData(GetNextNodeData(nodes.FirstOrDefault(node => node.nodeGUID == dialogueData.entryPointNodeGuid)));

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
        ReadNodeData(nextData)();
    }

    public Action ReadNodeData(NodeData nodeData)
    {
        _currentNodeData = nodeData;
        if (nodeData == null)
            return () => { };
        switch (nodeData.nodeType)
        {

            case NodeType.Dialogue:
                DialogueNodeData dialogueData = nodeData as DialogueNodeData;
                return () =>
                {
                    SendNewMessage(dialogueData.dialogueText);
                    ReadNextNode(nodeData);
                };
            case NodeType.Choice:
                ChoiceNodeData choiceData = nodeData as ChoiceNodeData;
                return () => DisplayChoices(choiceData.outputs);
            default:
                return () => { };
        }
        
    }

    void SendNewMessage(string text)
    {
        Debug.Log(text);
        
    }

    void DisplayChoices(List<OutputData> choices)
    {
        //=> More like display one choice to test
        int id = UnityEngine.Random.Range(0, choices.Count);
        var choice = choices[id];
        Debug.Log("Choix " + id + ": " + choices[id].portValue);
    }



    IEnumerator Conversation()
    {
        
        yield return new WaitForSeconds(1f);
    }


}
