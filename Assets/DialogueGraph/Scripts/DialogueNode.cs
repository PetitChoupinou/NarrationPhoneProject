using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueNode : BaseNode {

    [Input] public int input;
    [Output] public int output;

    // A modifier si on ajoute un SpeakerData
    public string speakerName;
    public string dialogueText;
    //public Sprite dialogueImage;

    public override string GetData()
    {
        return "Dialogue/" + speakerName + "/" + dialogueText;
    }
}