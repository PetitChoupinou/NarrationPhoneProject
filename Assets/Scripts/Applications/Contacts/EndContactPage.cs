using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class EndContactPage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private string _iD;
    private GameObject _noteButton;
    [SerializeField]private GameObject _warningPanel;

    private ContactApp _contactApp;
    private MessageApp _messageApp;
    private PhoneApp _phoneApp;
   
    private TMP_Text _preview;
    private TMP_Text _headerText;
    [SerializeField] private Dictionary<CharaEmotion, Sprite> _charaEmotions = new Dictionary<CharaEmotion, Sprite>();

    [SerializeField] private TMP_Text _content;
    [SerializeField] private Image _profilPic;
    public string ID { get => _iD; }
   
    private void OnEnable()
    {
        FindAnyObjectByType<ContactApp>().CurrentContact = gameObject;
        if (_headerText)
            _headerText.text = _iD;
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
    }
    public void SetUp(string title, string num, GameObject button, TMP_Text headerText,Dictionary<CharaEmotion,Sprite> profilePics)
    {
        _iD = title;
        _noteButton = button;
        _headerText = headerText;
        _preview = _noteButton.GetComponent<ContactAppButton>().Preview;
        _content.text = num;
        _charaEmotions = profilePics;
        ChangeEmotion(CharaEmotion.Base);
        _profilPic.color=Color.white;
        ChangePreview(num);
       _contactApp= (ContactApp)AppManager.Instance.GetApplication(ApplicationType.Contacts);
       
    }
    public void ChangePreview(string text)
    {
        _preview.text = text;
    }
    public void WarningButton()
    {
        _warningPanel.SetActive(true);
    }
    public void Cancel()
    {
        _warningPanel.SetActive(false);
    }
    public void MessageButton()
    {
        if (_messageApp == null)
        {
            _messageApp = (MessageApp)AppManager.Instance.GetApplication(ApplicationType.Messages);
        }
        _contactApp.CloseCurrent();
        _contactApp.CloseApp();
        _messageApp.GetComponent<Canvas>().enabled=true;
        _messageApp.AddEndConversation();
        Discussion discussion = _messageApp.GetDiscussion(_iD);
        discussion.MessageButton.GetComponent<InAppButton>().OnButtonClicked();
    }

    public void CallButton()
    {
        if (_phoneApp == null)
        {
            _phoneApp = (PhoneApp)AppManager.Instance.GetApplication(ApplicationType.Telephone);
        }
        _contactApp.CloseCurrent();
        _contactApp.CloseApp();
        _phoneApp.GetComponent<Canvas>().enabled = true;
        _phoneApp.AddToCurrentNbr(_content.text);
        _phoneApp.Call();
    }
    private void ChangeEmotion(CharaEmotion Emotion)
    {
        if (_charaEmotions.ContainsKey(Emotion))
        {
            _profilPic.sprite = _charaEmotions[Emotion];
        }
    }
}
