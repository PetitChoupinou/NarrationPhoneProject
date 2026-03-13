using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageTextBase : MonoBehaviour
{
    private HorizontalLayoutGroup _layoutGroup;
    private TMP_Text _message;
    [SerializeField] private bool _isNPCMsg;
    [SerializeField] int _maxMsgWidth=15;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _message = GetComponentInChildren<TMP_Text>();
        _layoutGroup = GetComponent<HorizontalLayoutGroup>();
    }
    void Start()
    {
        if (_isNPCMsg)
        {
            _layoutGroup.childAlignment=TextAnchor.UpperRight;
            SetTextMsg("ahhhhhh\nhhhhhhhhhaouifhozlhafiughghahhhheAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        }
        else
        {
            _layoutGroup.childAlignment = TextAnchor.UpperLeft;
            SetTextMsg("alors bonjour d'abords");
        }
    }

    public void SetTextMsg(string msg)
    {
        if (msg.Length > _maxMsgWidth)
        {
            print(msg);
           msg= AddLineReturn(msg);
            print(msg);
        }
        _message.text = msg;
    }
    public string AddLineReturn(string text)
    {
        int lastSpace = 0;
        int offset = 0;
        string returnText=text;
        int j = 1;
        for (int i = 1; i < text.Length; i++)
        {
            print(text[i]);
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
                print("bitch" + text[i]);
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
