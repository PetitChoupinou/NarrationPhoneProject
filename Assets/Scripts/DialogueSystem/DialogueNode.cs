using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNode : BaseNode
{
    public string dialogueText;
    public TextField textField;

    public void UpdateTextFieldValue()
    {
        textField.SetValueWithoutNotify(dialogueText);
    }
}
