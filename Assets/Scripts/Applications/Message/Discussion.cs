using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class Discussion : MonoBehaviour
{
    private TMP_Text _lastMessage;
    private string _iD;
    private TMP_Text _preview;
    private TMP_Text _headerText;
    private GameObject _messageButton;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _messagePrefab;
    [SerializeField] private GameObject _choicePrefab;
    [SerializeField] private GameObject _choicePanel;
    private bool _canChoose=false;
    private List<string> _choices = new List<string>();

    private DialogueDataReader _dialogueDataReader;

    public string ID { get => _iD;}
    public bool CanChoose { get => _canChoose; set => _canChoose = value; }
    public DialogueDataReader DialogueDataReader { get => _dialogueDataReader; set => _dialogueDataReader = value; }

    private void OnEnable()
    {
        FindAnyObjectByType<MessageApp>().CurrentConv = gameObject;
        if(_headerText)
        _headerText.text = _iD;
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
    }

    public void SetUp(string name,SentText[] texts,GameObject button, TMP_Text headerText)
    {
        DialogueDataReader = GetComponent<DialogueDataReader>();
        _iD = name;
        DialogueDataReader.CharacterID = name;
        _headerText = headerText;
        _messageButton = button;
        _preview = _messageButton.GetComponent<InAppButton>().Preview;
        /*if (texts.Length<=0) return;
        for (int i = 0; i < texts.Length; i++)
        {
            AddMessage(texts[i].Text, texts[i].isNPC);
        }
        if (_lastMessage)
        {
            ChangePreview(_lastMessage.text);
        }
        else
        {
            _preview.text = "";
        }*/
    }

    public void AddMessage(string text,bool isNPC)
    {
        GameObject newMessage = Instantiate(_messagePrefab, _content.transform);
        MessageTextBase message = newMessage.GetComponent<MessageTextBase>();
        message.SetTextMsg(text);
        message.SetIsNPC(isNPC);
        _lastMessage = message.Message;
        ChangePreview(text);
        StartCoroutine(MessageApplyResize(newMessage));
    }
    public void TriggerChoice(List<string> choices)
    {
        _canChoose = true;
        
        _choices.AddRange(choices);
        _choicePanel.SetActive(true);
        for (int i=0;i<choices.Count; i++)
        {
            GameObject choice=Instantiate(_choicePrefab, _choicePanel.transform);
            choice.GetComponentInChildren<TMP_Text>().text = choices[i];
            //GameObject choice=Instantiate(_choicePrefab, transform);
            _choicePrefab.GetComponent<RectTransform>().localPosition += new Vector3(0,40,0);
        }

    }
    public void AddMessage(string text)
    {
        StopAllCoroutines();
        GameObject newMessage = Instantiate(_messagePrefab, _content.transform);
        MessageTextBase message = newMessage.GetComponent<MessageTextBase>();
        message.SetTextMsg(text);
        message.SetIsNPC(false);
        StartCoroutine(MessageApplyResize(newMessage));
        _lastMessage = message.Message;
        ChangePreview(text);
    }
    public void ChangePreview(string text)
    {
        string previewText = "";
        if (text.Length > 15)
        {
            for (int i = 0; i < 12; i++)
            {
                previewText += text[i];
            }
            previewText += "...";
        }
        else previewText = text;
        _preview.text = previewText;
    }
    IEnumerator MessageApplyResize(GameObject newMessage)
    {
        yield return new WaitForSeconds(.01f);
        newMessage.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
        newMessage.GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
        yield return null;
    }
    public void StartChoice()
    {
        if (_canChoose)
        {
            _choicePanel.SetActive(true);
        }
    }
    public void Choose(string msg)
    {
        _choicePanel.SetActive(false);
        _canChoose = false;
        _choices.Clear();
        _dialogueDataReader.MakeChoice(msg);
        //fait ce que tu veux

    }
}
