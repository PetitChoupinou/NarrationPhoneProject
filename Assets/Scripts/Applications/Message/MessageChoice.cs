using TMPro;
using UnityEngine;

public class MessageChoice : MonoBehaviour
{
    [SerializeField] TMP_Text text; 
    public void OnPressed()
    {
        print(text.text);
        var currentConv = FindFirstObjectByType<MessageApp>();
        Debug.Log("Current conv: " + currentConv);
        Debug.Log("Discussion: " + currentConv.CurrentConv.GetComponent<Discussion>());
        currentConv.CurrentConv.GetComponent<Discussion>().Choose(text.text);
    }
    
}
