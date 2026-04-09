using UnityEngine;
using UnityEngine.UIElements;

public class ThinkingNode : BaseNode
{
    public TextField textField;
    public string text;

    public void UpdateTextFieldValue()
    {
        textField.SetValueWithoutNotify(text);
    }
}