using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TCG.Core.Dialogues;

public class MessageTextBase : MonoBehaviour
{
    private HorizontalLayoutGroup _layoutGroup;
    private TMP_Text _message;
    [SerializeField] private bool _isNPCMsg;
    [SerializeField] private UITextTyperMsg _textTyper;
    [SerializeField] int _maxMsgWidth=15;

    public TMP_Text Message { get => _message;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _layoutGroup = GetComponent<HorizontalLayoutGroup>();

    }
    public void SetIsNPC(bool isNPC)
    {
        _isNPCMsg = isNPC;
        if (_isNPCMsg)
        {
            _layoutGroup.childAlignment = TextAnchor.UpperLeft;
        }
        else
        {
            _layoutGroup.childAlignment = TextAnchor.UpperRight;
        }
    }

    public void SetTextMsg(string msg)
    {
        if (msg.Length > _maxMsgWidth)
        {
           msg= AddLineReturn(msg);
        }
        _textTyper.ReadText(msg);
    }
    public string AddLineReturn(string text)
    {
        int lastSpace = 0;
        int offset = 0;
        string returnText=text;
        int j = 1;
        for (int i = 1; i < text.Length; i++)
        {
            if(text[i] == '\n')
            {
                j = 1;
                continue;
            }
            if (text[i]==' ')
            {
                lastSpace=i;
            }
            if (j == _maxMsgWidth)
            {
                //print("bitch" + text[i]);
                if (lastSpace == 0|| lastSpace+ _maxMsgWidth < i)
                {
                    returnText= returnText.Insert(i+offset, "\n");
                }
                else
                {
                    returnText = returnText.Insert(lastSpace+offset, "\n");
                }
                offset += 2;
                j = 0;
            }
            j++;
        }
        return returnText;
    }
}
