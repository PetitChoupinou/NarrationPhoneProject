using TMPro;
using UnityEngine;

public class MessageChoice : MonoBehaviour
{
    [SerializeField] TMP_Text text; 
    public void OnPressed()
    {
        FindFirstObjectByType<MessageApp>().CurrentConv.GetComponent<Discussion>().AddMessage(text.text,false);
        FindFirstObjectByType<MessageApp>().CurrentConv.GetComponent<Discussion>().CanChoose=false;
    }
    
}
