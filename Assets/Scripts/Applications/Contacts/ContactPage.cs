using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    
    [SerializeField] private TMP_Text _content;
    [SerializeField] private Image _profilPic;
    [SerializeField] private Color _relationshipGoodColor;
    [SerializeField] private Color _relationshipBadColor;
    [SerializeField] private Image _relationBackground;
    public string ID { get => _iD; }
    public float Relation { get => _relation;
        set {
           //_relationBackground.color = Color.Lerp(_relationshipBadColor, _relationshipGoodColor, (value + 10.0f) / 20.0f);
            _relation = value;
        }}
   
    private void OnEnable()
    {
        FindAnyObjectByType<ContactApp>().CurrentContact = gameObject;
        if (_headerText)
            _headerText.text = _iD;
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
    }
    public void SetUp(string title, string num,int relation, GameObject button, TMP_Text headerText,Sprite profilePic)
    {
        _iD = title;
        _noteButton = button;
        Relation=relation;
        _headerText = headerText;
        _preview = _noteButton.GetComponent<ContactAppButton>().Preview;
        _content.text = num;
        _profilPic.sprite = profilePic;
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
}
