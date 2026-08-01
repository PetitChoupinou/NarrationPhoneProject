using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Discussion : MonoBehaviour
{
    private TMP_Text _lastMessage;
    private string _iD;
    private TMP_Text _preview;
    private TMP_Text _headerText;
    private GameObject _messageButton;
    private MessageApp _messageApp;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _messagePrefab;
    [SerializeField] private GameObject _linkPrefab;
    [SerializeField] private GameObject _choicePrefab;
    [SerializeField] private GameObject _choicePanel;
    [SerializeField] private bool _isEnabled;
    private Queue<PendingMsg> _pendingMsgs=new Queue<PendingMsg>();
    private bool _canChoose=false;
    private List<string> _choices = new List<string>();
    private List<GameObject> _choiceButtons = new List<GameObject>();

    private DialogueDataReader _dialogueDataReader;

    public string ID { get => _iD;}
    public bool CanChoose { get => _canChoose; set => _canChoose = value; }
    public DialogueDataReader DialogueDataReader { get => _dialogueDataReader; set => _dialogueDataReader = value; }
    public GameObject MessageButton { get => _messageButton;}
    public bool IsEnabled { get => _isEnabled; set => _isEnabled = value; }

    private void Start()
    {
        _messageApp= FindAnyObjectByType<MessageApp>();
    }
    public void Enable()
    {
        print(_messageApp);
        _isEnabled = true;
        if(_headerText)
        _headerText.text = _iD;
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
        _messageApp.SetCurrentConv(transform.gameObject);

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
        if (AppManager.Instance.GetApplication(ApplicationType.Map)&&PhoneManager.Instance.CurrentLocation.networkState == NetworkState.Bad)
        {
            _pendingMsgs.Enqueue(new PendingMsg(isNPC, text));
            return;
        }
        GameObject newMessage = Instantiate(_messagePrefab, _content.transform);
        MessageTextBase message = newMessage.GetComponent<MessageTextBase>();
        message.SetIsNPC(isNPC);
        message.SetTextMsg(text);
        _lastMessage = message.Message;
        ChangePreview(text);
        StartCoroutine(MessageApplyResize(newMessage));
        if(_messageApp.CurrentConv != this.gameObject )
        {
            NotificationManager.Instance.SendNotifText(_preview.text, _iD);
        }
    }
    public void AddLinkTo(ApplicationType type)
    {
        if (AppManager.Instance.GetApplication(ApplicationType.Map) && PhoneManager.Instance.CurrentLocation.networkState == NetworkState.Bad)
        {

            _pendingMsgs.Enqueue(new PendingMsg(type));
            return;
        }
        GameObject newMessage = Instantiate(_linkPrefab, _content.transform);
        MessageLink message = newMessage.GetComponent<MessageLink>();
        message.SetLinkMsg(type);
        ChangePreview("", true);
        StartCoroutine(MessageApplyResize(newMessage));
        if (_messageApp.CurrentConv != this.gameObject)
        {
            NotificationManager.Instance.SendNotifText(_preview.text, _iD);
        }

    }
    public void TriggerChoice(List<string> choices)
    {
        _canChoose = true;
        
        _choices.AddRange(choices);
        _choicePanel.SetActive(true);
        for (int i=0;i<choices.Count; i++)
        {
            GameObject choice=Instantiate(_choicePrefab, _choicePanel.transform.GetChild(0));
            _choiceButtons.Add(choice);
            choice.GetComponentInChildren<TMP_Text>().text = choices[i];
            //GameObject choice=Instantiate(_choicePrefab, transform);
            _choicePrefab.GetComponent<RectTransform>().localPosition += new Vector3(0,40,0);
            if (!DialogueDataReader.IsChoicePossible(choices[i]))
            {
                choice.GetComponent<Image>().color = Color.red;
                choice.GetComponent<Button>().interactable = false;
            }
        }

    }
    public void ChangePreview(string text,bool isDl=false)
    {
        if (isDl)
        {
            _preview.text = "download";
            return;
        }
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
        newMessage.SetActive(false);
        yield return new WaitForSeconds(.001f);
        newMessage.SetActive(true);
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
        if (AppManager.Instance.GetApplication(ApplicationType.Map)&&PhoneManager.Instance.CurrentLocation.networkState == NetworkState.Bad)
        {
            PhoneManager.Instance.CreateThought("Hmm pas de réseaux.");
            return;
        }
        _canChoose = false;
        foreach(GameObject buttton in _choiceButtons)
        {
            Destroy(buttton);
        }
        _choiceButtons.Clear();
        _choices.Clear();
        _dialogueDataReader.MakeChoice(msg);
    }

    void ClearChoices()
    {
        var choiceButtons = _choicePanel.GetComponentsInChildren<MessageChoice>();
        foreach(var choice in choiceButtons)
        {
            Destroy(choice.gameObject);
        }
        _choices.Clear();
    }

    public void CreateThought(string thought)
    {
        PhoneManager.Instance.CreateThought(thought);
    }
    public void DequeuPendingMessages()
    {
        while (_pendingMsgs.Count > 0)
        {
            
            PendingMsg current = _pendingMsgs.Dequeue();
            if (current.isDownload)
            {
                AddLinkTo(current.app);
            }
            else
            {
                AddMessage(current.text, current.isNPC);
            }
        }
    }
}
public class PendingMsg
{
    public bool isNPC;
    public bool isDownload;
    public ApplicationType app;
    public string text;

    public PendingMsg(bool isNPC, string text)
    {
        this.isNPC = isNPC;
        this.text = text;
        this.isDownload = false;
    }
    public PendingMsg(ApplicationType type)
    {
        this.isDownload=true;
        this.app = type;
    }

}
