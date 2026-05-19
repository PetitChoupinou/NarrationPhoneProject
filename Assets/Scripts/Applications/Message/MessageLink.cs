using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TCG.Core.Dialogues;

public class MessageLink : MonoBehaviour
{
    private HorizontalLayoutGroup _layoutGroup;
    [SerializeField]private Image  _link;
    private ApplicationType _applicationType;

    private MessageApp _messageApp;
    private InternetApp _internetApp;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _layoutGroup = GetComponent<HorizontalLayoutGroup>();
        _layoutGroup.childAlignment = TextAnchor.UpperLeft;
        AppManager appManager = AppManager.Instance;
        _messageApp=appManager.GetApplication(ApplicationType.Messages).GetComponent<MessageApp>();
        _internetApp = appManager.GetApplication(ApplicationType.Internet).GetComponent<InternetApp>();
    }

    public void SetLinkMsg(ApplicationType type,Sprite spr=null)
    {
       _applicationType = type;
        _link.sprite = null;
       
    }
    public void OnClickedLink()
    {
        if(_internetApp == null)
        {
            _internetApp = (InternetApp)AppManager.Instance.GetApplication(ApplicationType.Internet);
        }
        _messageApp.CloseCurrent();
        _messageApp.CloseApp();
        AppManager.Instance.GetApplication(ApplicationType.Internet).GetComponent<InternetApp>().setDlPage("https:/safeDLtktçapasse.to", _applicationType);
        _internetApp.OnActivated();
        _internetApp.GetComponent<Canvas>().enabled = true;
        
    }
}
