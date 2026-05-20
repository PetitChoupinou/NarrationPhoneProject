using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class Note : MonoBehaviour
{

    private string _iD;
    private GameObject _noteButton;
    private TMP_Text _preview;
    private TMP_Text _headerText;
    [SerializeField] private TMP_Text _content;
    public string ID { get => _iD; }

 
    private void OnEnable()
    {
        FindAnyObjectByType<NoteApp>().CurrentNote = gameObject;
        if (_headerText)
            _headerText.text = _iD;
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
    }
    public void SetUp(string title,string content,GameObject button,TMP_Text headerText,bool hasPreview = true)
    {
         _iD = title;
        _noteButton = button;
        _headerText = headerText;
       
        _content.text = content;
        if (hasPreview)
        {
            _preview = _noteButton.GetComponent<InAppButton>().Preview;
            ChangePreview(content);
        }
    }

    public void AddNote(string content)
    {
        _content.text += "\r\n\r\n" + content;
    }
    public void ChangePreview(string text)
    {
        string previewText = "";
        if (text.Length > 30)
        {
            for (int i = 0; i < 27; i++)
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

}
