using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Note : MonoBehaviour
{
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

}
