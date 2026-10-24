using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TCG.Core.Dialogues;
using System.Collections;

public class MessageTextBase : MonoBehaviour
{
    private HorizontalLayoutGroup _layoutGroup;
    private TMP_Text _message;
    [SerializeField] private bool _isNPCMsg;
    [SerializeField] private GameObject _messagePrefab;
    [SerializeField] private UITextTyperMsg _textTyper;
    [SerializeField] private Image _bgImage;
    [SerializeField] int _maxMsgWidth=15;
    [SerializeField] private Image _readMark;
    [SerializeField] private Sprite _sent;
    [SerializeField] private Sprite _recieved;
    [SerializeField] private Sprite _read;
    private WaitForSecondsRealtime wait = new WaitForSecondsRealtime(.5f);
    public TMP_Text Message { get => _message;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _layoutGroup = GetComponent<HorizontalLayoutGroup>();
        DarkModeManager.Instance.SwitchObjectToDarkMode(gameObject.transform.GetChild(0).GetComponent<Image>());

    }
    public void SetIsNPC(bool isNPC, Sprite bgPlayer, Sprite bgNPC)
    {
        _isNPCMsg = isNPC;
        if (_isNPCMsg)
        {
            Destroy(_readMark.gameObject);
            _layoutGroup.childAlignment = TextAnchor.LowerLeft;
            _bgImage.sprite = bgNPC;
        }
        else
        {
            _layoutGroup.childAlignment = TextAnchor.LowerRight;
            _bgImage.sprite = bgPlayer;
            Invoke("ChangeMarkToSent",0);
            Invoke("ChangeMarkToReceived",.5f);
            Invoke("ChangeMarkToRead",1.5f);
        }
    }

    public void SetTextMsg(string msg)
    {
        GameObject message = GameObject.Instantiate(_messagePrefab, transform.GetChild(1));
        _message = message.GetComponent<TMP_Text>();
        _textTyper.TextField = _message.GetComponent<TextMeshProUGUI>();
        _textTyper._text = _message;
        if (msg.Length > _maxMsgWidth)
        {
            msg = AddLineReturn(msg);
        }
        _textTyper.ReadText(msg);
        DarkModeManager.Instance.SwitchTextToDarkMode(_textTyper.TextField);
    }


    public string AddLineReturn(string text)
    {
        int lastSpace = 0;
        string returnText = text;
        int j = 1;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
            {
                lastSpace = i;
            }
            if (i == _maxMsgWidth * j - 1)
            {
                if (i - lastSpace < _maxMsgWidth)
                {
                    returnText = returnText.Insert(lastSpace + 1, "\n");
                }
                else
                {
                    returnText = returnText.Insert(i + 1, "\n");
                }
                returnText = returnText.Replace(" \n", "\n");

                j++;
            }
        }
        return returnText;
    }
  private void ChangeMarkToSent()
    {
        _readMark.sprite = _sent;
    }
    private void ChangeMarkToReceived()
    {
        _readMark.sprite = _recieved;
    }
    private void ChangeMarkToRead()
    {
        _readMark.sprite = _read;
    }
}
