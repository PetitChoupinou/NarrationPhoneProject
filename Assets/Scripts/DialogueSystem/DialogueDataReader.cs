using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueDataReader : MonoBehaviour
{
    //TODO: Read the dialogue data from the file and return a list of actions (SendMessage, WaitSomeTime, etc.)

    public DialogueData dialogueData;

    private void Start()
    {
        List<NodeData> nodes = dialogueData.nodes;
        foreach (var nodeData in dialogueData.nodes)
        {
            Action action = ReadNodeData(nodeData.nodeInfos);
            action.Invoke();
        }
    }

    public Action ReadNodeData(BaseNode nodeData)
    {
        switch (nodeData.nodeType)
        {
                
            case NodeType.Dialogue:
                DialogueNode dialogueData = nodeData as DialogueNode;
                return () => SendMessage(dialogueData.dialogueText);
            case NodeType.Choice:
                ChoiceNode choiceData = nodeData as ChoiceNode;
                return () => DisplayChoices(choiceData.choices);
            default:
                return () => { };
        }
    }

    void SendMessage(string text)
    {
        Debug.Log(text);
    }

    void DisplayChoices(List<ChoiceInfos> choices)
    {
        for (int i = 0; i < choices.Count; i++)
        {
            Debug.Log("Choix " + i +": " + choices[i].choiceText);
        }
    }


    IEnumerator Conversation()
    {
        
        yield return new WaitForSeconds(1f);
    }


}
