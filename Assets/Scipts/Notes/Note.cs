using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class Note : MonoBehaviour
{

    
    private GameObject _noteButton;
    [SerializeField] private TMP_Text _preview;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _content;

    public  void SetUp(string title,string content)
    {
        _title.text = title;
        _content.text = content;
    }

    public void AddNote(string content)
    {
        _content.text += "\r\n" + content;
    }
    public void ChangePreview(string text)
    {
        string previewText = "";
        if (text.Length > 15)
        {
            for (int i = 0; i < 12; i++)
            {
                previewText += text[i];
                if (text[i] == '\n')
                {
                    break;
                }
            }
            previewText += "...";
        }
        else previewText = text;
        _preview.text = previewText;
    }
    public void CloseDiscussion()
    {
        _noteButton.GetComponent<ButtonMsg>().Parent.SetActive(true);
        gameObject.SetActive(false);
    }

}
