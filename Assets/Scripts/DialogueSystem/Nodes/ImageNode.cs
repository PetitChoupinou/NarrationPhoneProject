#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ImageNode : BaseNode
{
    public DropdownField senderField;
    public bool isNPC;
    public Image imageField;
    [SerializeField] public Sprite imageSprite;
    public float timerSending;
    private FloatField _timeField;

    public FloatField TimeField { get => _timeField; set => _timeField = value; }


    public void UpdateSenderField()
    {
        senderField.SetValueWithoutNotify(isNPC ? "NPC" : "Player");
    }

    public void UpdateImageField()
    {
        imageField.sprite = imageSprite;
    }
    
}
#endif