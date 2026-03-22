using TMPro;
using UnityEngine;

public class MessageChoice : MonoBehaviour
{
    [SerializeField] TMP_Text text; 
    public void OnPressed()
    {
        print(text);
        FindFirstObjectByType<MessageApp>().CurrentConv.GetComponent<Discussion>().Choose(text.text);
    }
    
}
