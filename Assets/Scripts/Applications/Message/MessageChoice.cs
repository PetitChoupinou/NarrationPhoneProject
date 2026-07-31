using TMPro;
using UnityEngine;

public class MessageChoice : MonoBehaviour
{
    [SerializeField] TMP_Text text; 
    public void OnPressed()
    {
        print(text.text);
        var currentConv = FindFirstObjectByType<MessageApp>();
        currentConv.CurrentConv.GetComponent<Discussion>().Choose(text.text);
    }
    
}
