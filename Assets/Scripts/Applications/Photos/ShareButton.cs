using UnityEngine;
using UnityEngine.UI;

public class ShareButton : MonoBehaviour
{
    private Discussion _discussion;
    private MessageApp _messageApp;
    private PhotoApp _photoApp;
    [SerializeField] Image _visu;
    public void Setup(Discussion discussion,Sprite visu)
    {
        _discussion = discussion;
        _messageApp=   (MessageApp)AppManager.Instance.GetApplication(ApplicationType.Messages);
        _photoApp=   (PhotoApp)AppManager.Instance.GetApplication(ApplicationType.Photos);
        _visu.sprite = visu;
    }

    public void Share()
    {
        if (_messageApp == null)
        {
            _messageApp = (MessageApp)AppManager.Instance.GetApplication(ApplicationType.Messages);
        }
        _photoApp.CloseCurrent();
        _photoApp.CloseCurrent();
        _photoApp.CloseApp();
        _messageApp.GetComponent<Canvas>().enabled = true;
        _discussion.MessageButton.GetComponent<InAppButton>().OnButtonClicked();
        //ajouter lappel de la fonction pour envoyer le message photo
    }
}
