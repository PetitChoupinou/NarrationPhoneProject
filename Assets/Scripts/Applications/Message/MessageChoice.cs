using TCG.Core.Dialogues;
using TMPro;
using UnityEngine;

public class MessageChoice : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] UITextTyperMsg  textTyper;
    public void OnPressed()
    {
        print(text.text);
        var messageApp = FindFirstObjectByType<MessageApp>();
        messageApp.SetCurrentConv(messageApp.GetCurrentDiscussion().gameObject);
        if (messageApp.CurrentConv==null)
        {
            return; // il y a un problème si ça passe par là!
        }
        messageApp.CurrentConv.GetComponent<Discussion>().Choose(text.text);
    }
    
}
