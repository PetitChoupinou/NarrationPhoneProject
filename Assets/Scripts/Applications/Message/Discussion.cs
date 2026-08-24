using System;
using System.Collections;
using System.Collections.Generic;
using TCG.Core.Dialogues;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Discussion : MonoBehaviour
{
    private TMP_Text _lastMessage;
    private string _iD;
    private TMP_Text _preview;
    private TMP_Text _headerText;
    private GameObject _messageButton;
    private MessageApp _messageApp;
    private Sprite _backgroundImage;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _messagePrefab;
    [SerializeField] private GameObject _linkPrefab;
    [SerializeField] private GameObject _choicePrefab;
    [SerializeField] private GameObject _choicePanel;
    [SerializeField] private SendingButton _sendingButton;
    [SerializeField] private bool _isEnabled;
    [SerializeField] private Image _charaVisu;
    [SerializeField] private Dictionary<string,Sprite> _charaEmotions=new Dictionary<string, Sprite>();
   [SerializeField] private Vector3 _charaVisuBasePosition;


    private Queue<PendingMsg> _pendingMsgs=new Queue<PendingMsg>();
    private bool _canChoose=false;
    private List<string> _choices = new List<string>();
    private List<GameObject> _choiceButtons = new List<GameObject>();
    private ScrollRect _scrollRect;

    private DialogueDataReader _dialogueDataReader;

    #region Relationship Feedback
    [SerializeField] private Image _relationFeedback;
    [SerializeField] private Material _positifRel;
    [SerializeField] private Material _negatiifRel;
    [SerializeField] private float  _feedbackDuration;
    #endregion
    public string ID { get => _iD;}
    public bool CanChoose { get => _canChoose; set => _canChoose = value; }
    public DialogueDataReader DialogueDataReader { get => _dialogueDataReader; set => _dialogueDataReader = value; }
    public GameObject MessageButton { get => _messageButton;}
    public bool IsEnabled { get => _isEnabled; set => _isEnabled = value; }

    private void Start()
    {
        _messageApp= FindAnyObjectByType<MessageApp>();
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _charaVisuBasePosition = _charaVisu.GetComponent<RectTransform>().anchoredPosition;
    }
#if UNITY_EDITOR
    bool isCharaVisuSideMode=true;
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            print("f");
            isCharaVisuSideMode = !isCharaVisuSideMode;
        }
    }
#endif
    /// <summary>
    /// Not OnEnable as it is not disabled when not on it
    /// </summary>
    public void Enable()
    {
        if (_messageApp == null) _messageApp = FindAnyObjectByType<MessageApp>();
        print(_messageApp);
        _messageApp.SetBackground(_backgroundImage);
        _isEnabled = true;
        if(_headerText)
        _headerText.text = _iD;
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
        _messageApp.SetCurrentConv(transform.gameObject);

    }
    /// <summary>
    /// Set the discussions elements up
    /// </summary>
    /// <param name="name">Conversation Name</param>
    /// <param name="texts">deprecated</param>
    /// <param name="button">button to discussion</param>
    /// <param name="headerText">Text field</param>
    /// <param name="background">conversation background image</param>
    public void SetUp(string name, SentText[] texts, GameObject button, TMP_Text headerText, Sprite background, Dictionary<string, Sprite> chara)
    {
        DialogueDataReader = GetComponent<DialogueDataReader>();
        _iD = name;
        DialogueDataReader.CharacterID = name;
        _headerText = headerText;
        _messageButton = button;
        _preview = _messageButton.GetComponent<InAppButton>().Preview;
        _backgroundImage = background;
        _charaEmotions = chara;
        ChangeEmotion("Base");
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
    /// <summary>
    /// Adds a message to the conversation
    /// </summary>
    /// <param name="text">message</param>
    /// <param name="isNPC">allow to put messages in the right spot</param>
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
        newMessage.transform.localScale = Vector3.zero;
        StartCoroutine(MessageApplyResize(newMessage));
        if(_messageApp==null) _messageApp = FindAnyObjectByType<MessageApp>();
        if (_messageApp.CurrentConv != gameObject )
        {
            NotificationManager.Instance.SendNotifText(_preview.text, _iD);
        }
        Transform visuTransform = _charaVisu.transform;
#if UNITY_EDITOR
        if (isNPC)
        {
            if(!isCharaVisuSideMode)
                visuTransform.SetAsLastSibling();
            else
            {
                visuTransform.localScale=new Vector3(1,1,1);
                _charaVisu.GetComponent<RectTransform>().anchoredPosition=new Vector3(_charaVisuBasePosition.x,_charaVisuBasePosition.y,_charaVisuBasePosition.z);
            }

        }
        else {

            if(!isCharaVisuSideMode)
                visuTransform.SetAsFirstSibling();
            else
            {
                visuTransform.localScale=new Vector3(-1,1,1);
                _charaVisu.GetComponent<RectTransform>().anchoredPosition = new Vector3(-_charaVisuBasePosition.x,_charaVisuBasePosition.y,_charaVisuBasePosition.z);
            }
        }
#else
        if (isNPC)
        {
               visuTransform.localScale=new Vector3(1,1,1);
                _charaVisu.GetComponent<RectTransform>().anchoredPosition=new Vector3(_charaVisuBasePosition.x,_charaVisuBasePosition.y,_charaVisuBasePosition.z);
        }
        else 
        {
                visuTransform.localScale=new Vector3(-1,1,1);
                _charaVisu.GetComponent<RectTransform>().anchoredPosition = new Vector3(-_charaVisuBasePosition.x,_charaVisuBasePosition.y,_charaVisuBasePosition.z);  
        }
#endif

    }
    /// <summary>
    /// Link to add an app to the phone
    /// </summary>
    /// <param name="type">What app is downloadable from the link</param>
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
        newMessage.transform.localScale = Vector3.zero;
        StartCoroutine(MessageApplyResize(newMessage));
        if (_messageApp == null) _messageApp = FindAnyObjectByType<MessageApp>();
        if (_messageApp.CurrentConv != gameObject)
        {
            NotificationManager.Instance.SendNotifText(_preview.text, _iD);
        }

    }

    public void EnableSendingButton(Action sendingAction)
    {
        _sendingButton.EnableButton(sendingAction);
    }

    public void DisableSendingButton()
    {
        _sendingButton.DisableButton();
    }
    /// <summary>
    /// Add choices to choice panel
    /// </summary>
    /// <param name="choices">message previews</param>
    public void TriggerChoice(List<string> choices)
    {
        _canChoose = true;
        
        _choices.AddRange(choices);
        _choicePanel.SetActive(true);
        for (int i=0;i<choices.Count; i++)
        {
            GameObject choice=Instantiate(_choicePrefab, _choicePanel.transform.GetChild(0));
            _choiceButtons.Add(choice);
            choice.GetComponentInChildren<UITextTyperMsg>().ReadText(choices[i]);
            choice.GetComponent<MessageChoice>().value = choices[i];
            //GameObject choice=Instantiate(_choicePrefab, transform);
            _choicePrefab.GetComponent<RectTransform>().localPosition += new Vector3(0,40,0);
            if (!DialogueDataReader.IsChoicePossible(choices[i]))
            {
                choice.GetComponent<Image>().color = Color.red;
                choice.GetComponent<Button>().interactable = false;
            }
        }

    }
    /// <summary>
    /// Change preview on the base screen of the message app
    /// </summary>
    /// <param name="text">preview</param>
    /// <param name="isDl">is it a link</param>
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
    /// <summary>
    /// resize messages to make sure they fit well.
    /// </summary>
    /// <param name="newMessage"></param>
    /// <returns></returns>
    IEnumerator MessageApplyResize(GameObject newMessage)
    {
        yield return new WaitForSeconds(.1f);// going too low on this will make the resize fail sometimes

        bool isActive=false;
        if (_messageApp == null) _messageApp = FindAnyObjectByType<MessageApp>();
        if (_messageApp.CurrentConv == gameObject) isActive = true;
        newMessage.GetComponentInChildren<HorizontalLayoutGroup>().childControlHeight = false;
        newMessage.GetComponentInChildren<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
        newMessage.GetComponentInChildren<HorizontalLayoutGroup>().CalculateLayoutInputVertical();
       
        newMessage.SetActive(false);
       
        yield return new WaitForSeconds(.001f);

        if (isActive)
        {
            _content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            _content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputHorizontal();
        }
        newMessage.SetActive(true);
        newMessage.GetComponentInChildren<HorizontalLayoutGroup>().childControlHeight = true;
        newMessage.GetComponentInChildren<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
        newMessage.GetComponentInChildren<HorizontalLayoutGroup>().CalculateLayoutInputVertical();
        newMessage.transform.localScale = Vector3.one;
        yield return new WaitForEndOfFrameUnit();
        _scrollRect.verticalNormalizedPosition = 0;
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

    public void CreateThought(string thought)
    {
        PhoneManager.Instance.CreateThought(thought);
    }
    /// <summary>
    /// When out of network the incoming messages are stored to be added in order
    /// </summary>
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
    public void UpdateRelationhhip(float value)
    {
        bool isGood = false;
        if (value > 0) isGood = true;
        StartCoroutine(RelationshipFeedback(isGood));
    }

    public void ChangeEmotion(string Emotion)
    {
        if (_charaEmotions.ContainsKey(Emotion))
        {
            _charaVisu.sprite= _charaEmotions[Emotion];
        }
    }
    IEnumerator RelationshipFeedback(bool isGood)
    {
        if (isGood)
        {
            _relationFeedback.material = _positifRel;
            yield return null;
        }
        else
        {
            _relationFeedback.material = _negatiifRel;
            yield return null;

        }
        _relationFeedback.gameObject.SetActive(true);
        float duration=0;
        Color color = _relationFeedback.color;
        while (duration < .5f)
        {
            _relationFeedback.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0f, 1f, duration * 2));
            duration += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(_feedbackDuration);
        duration = 0;
        while (duration < .5f)
        {
            _relationFeedback.color = new Color(color.r, color.g, color.b, Mathf.Lerp(1f, 0f, duration * 2));
            duration += Time.deltaTime;
            yield return null;
        }
        _relationFeedback.gameObject.SetActive(false);
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
