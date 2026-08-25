using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ContactPage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private string _iD;
    private GameObject _noteButton;
    private float _relation;

    private ContactApp _contactApp;
    private MessageApp _messageApp;
    private PhoneApp _phoneApp;
   
    private TMP_Text _preview;
    private TMP_Text _headerText;
    private Material _materialInstance;
    [SerializeField] private Dictionary<CharaEmotion, Sprite> _charaEmotions = new Dictionary<CharaEmotion, Sprite>();

    [SerializeField] private TMP_Text _content;
    [SerializeField] private Image _profilPic;
    [SerializeField] private Image _relationBackground;
    
    [SerializeField] private Color _relationshipGoodColor;
    [SerializeField] private Color _relationshipBadColor;
    [SerializeField] private Color _outlineColor;
    [SerializeField,Range(.51f,.70f)] private float _outlineWidth;
    public string ID { get => _iD; }
    public float Relation { get => _relation;
        set {
            if (value < 0) value = 0;
            if (value >20) value = 20;
            float visibleValue=0;
            switch (value)
            {
                case 0:
                    visibleValue = 0;
                    break;
                case <5:
                    visibleValue = .20f;
                    break;
                case < 10:
                    visibleValue = .40f;
                    break;
                case < 15:
                    visibleValue = .60f;
                    break;
                case < 20:
                    visibleValue = .80f;
                    break;
                case 20:
                    visibleValue = 1;
                    break;
            }
            _materialInstance.SetFloat("_RelValue", visibleValue);
            _relation = value;
        }}
   
    private void OnEnable()
    {
        FindAnyObjectByType<ContactApp>().CurrentContact = gameObject;
        if (_headerText)
            _headerText.text = _iD;
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
    }
    public void SetUp(string title, string num,int relation, GameObject button, TMP_Text headerText,Dictionary<CharaEmotion,Sprite> profilePics)
    {
        _iD = title;
        _noteButton = button;
        _materialInstance = new Material(_relationBackground.material); ;
        _relationBackground.material = _materialInstance;
        _materialInstance.SetColor("_ColorGood", _relationshipGoodColor);
        _materialInstance.SetColor("_ColorBad", _relationshipBadColor);
        _materialInstance.SetColor("_OutlineColor", _outlineColor);
        _materialInstance.SetFloat("_RelValue", Relation / 20.0f);
        _materialInstance.SetFloat("_OutlineWidth", _outlineWidth);
        Relation =relation;
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

    public void MessageButton()
    {
        if (_messageApp == null)
        {
            _messageApp = (MessageApp)AppManager.Instance.GetApplication(ApplicationType.Messages);
        }
        _contactApp.CloseCurrent();
        _contactApp.CloseApp();
        _messageApp.GetComponent<Canvas>().enabled=true;
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
