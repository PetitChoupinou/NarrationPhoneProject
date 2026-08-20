using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TCG.Core.Dialogues;

public class MessageTextBase : MonoBehaviour
{
    private HorizontalLayoutGroup _layoutGroup;
    private TMP_Text _message;
    [SerializeField] private bool _isNPCMsg;
    [SerializeField] private GameObject _messagePrefab;
   [SerializeField]private UITextTyperMsg _textTyper;
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
        GameObject message=GameObject.Instantiate(_messagePrefab,transform.GetChild(0));
        _message = message.GetComponent<TMP_Text>();
        _textTyper.TextField =_message.GetComponent<TextMeshProUGUI>();
        _textTyper._text=_message;
        if (msg.Length > _maxMsgWidth)
        {
           msg= AddLineReturn(msg);
        }
        _textTyper.ReadText(msg);
    }
    public string AddLineReturn(string text)
    {
        int lastSpace = 0;
        string returnText=text;
        int j = 1;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i]==' ')
            {
                lastSpace=i;
            }
            if (i == _maxMsgWidth*j-1)
            {
                if (i-lastSpace< _maxMsgWidth)
                {
                    returnText= returnText.Insert(lastSpace+1, "\n");
                }
                else
                {
                    returnText = returnText.Insert(i+1, "\n");
                }
                returnText = returnText.Replace(" \n", "\n");

                j++;
            }
        }
        return returnText;
    }
}
