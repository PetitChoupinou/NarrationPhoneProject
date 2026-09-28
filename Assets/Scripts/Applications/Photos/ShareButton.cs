using UnityEngine;
using UnityEngine.UI;

public class ShareButton : MonoBehaviour
{
    private Discussion _discussion;
    private MessageApp _messageApp;
    private PhotoApp _photoApp;
    [SerializeField] Image _visu;
    [SerializeField] Material greyFilter;
    Material mat;

    private void Awake()
    {
        if (mat == null)
        {
            mat = new Material(greyFilter);
            _visu.material = mat;
        }
    }
    public void Setup(Discussion discussion,Sprite visu)
    {
        _discussion = discussion;
        _messageApp=   (MessageApp)AppManager.Instance.GetApplication(ApplicationType.Messages);
        _photoApp=   (PhotoApp)AppManager.Instance.GetApplication(ApplicationType.Photos);
        _visu.sprite = visu;
        _discussion.OnConversationStatutUpdate += SetIsInteractable;
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
        _discussion.DialogueDataReader.SendImageFromPhotoApp(_photoApp.Photo.sprite);
        
    }

    public void SetIsInteractable(bool isInteractable)
    {
        if(mat == null)
        {
            mat = new Material(greyFilter);
            _visu.material = mat;
        }
        Button button = GetComponent<Button>();
        if(isInteractable)
        {
            mat.SetFloat("_Saturation", 1);
            button.enabled = true;
        }
        else
        {
            mat.SetFloat("_Saturation", 0);
            button.enabled = false;
        }

    }
}
