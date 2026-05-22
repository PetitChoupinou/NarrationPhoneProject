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
    private GameObject _parent;
    [SerializeField] private TMP_Text _content;
    public string ID { get => _iD; }
    public TMP_Text Content { get => _content; }

    private void OnEnable()
    {
        if (_parent == null) return;
       if (_parent.TryGetComponent<NoteApp>(out NoteApp n))
        {
            n.CurrentNote = gameObject;
            PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
        }
        else
        {
            _parent.GetComponent<HackFolder>().CurrentFile = gameObject;
            PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.deep);
        }
        if (_headerText)
            _headerText.text = _iD;
      
    }
    public void SetUp(string title, string content, GameObject button, TMP_Text headerText, GameObject parent, bool previewTitle = false)
    {
         _iD = title;
        _noteButton = button;
        _headerText = headerText;
       _parent=parent;
        _content.text = content;
        _preview = _noteButton.GetComponent<InAppButton>().Preview;
        if (previewTitle)
        {
            ChangePreview(title);
        }
        else ChangePreview(content);
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
