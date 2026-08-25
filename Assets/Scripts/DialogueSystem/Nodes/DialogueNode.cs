#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNode : BaseNode
{
    public string dialogueText;
    public TextField textField;
    public DropdownField talkerField;
    public DropdownField emotionField;
    public bool isNPC;
    private FloatField _timeField;
    public CharaEmotion emotion; 
    public float timerSending;

    public FloatField TimeField { get => _timeField; set => _timeField = value; }

    public void UpdateTextFieldValue()
    {
        textField.SetValueWithoutNotify(dialogueText);
    }

    public void UpdateTalkerField()
    {
        talkerField.SetValueWithoutNotify(isNPC ? "NPC" : "Player");
    }

    public void UpdateEmotionField()
    {
        emotionField.SetValueWithoutNotify(emotion.ToString());
    }
}
#endif