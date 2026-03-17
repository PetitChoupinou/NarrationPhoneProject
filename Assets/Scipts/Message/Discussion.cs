using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class Discussion : MonoBehaviour
{
    [SerializeField] private TMP_Text _lastMessage;
    [SerializeField] private TMP_Text _preview;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _messagePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_lastMessage)
        {
            ChangePreview(_lastMessage.text);
        }
        else
        {
            _preview.text = "";
        }
    }

    public void AddMessage(string text,bool isNPC)
    {
        GameObject newMessage = Instantiate(_messagePrefab, _content.transform);
        MessageTextBase message = newMessage.GetComponent<MessageTextBase>();
        message.SetTextMsg(text);
        message.SetIsNPC(isNPC);
        _lastMessage = message.Message;
        ChangePreview(text);
    }
    public void AddMessage(string text)
    {
        StopAllCoroutines();
        GameObject newMessage = Instantiate(_messagePrefab, _content.transform);
        MessageTextBase message = newMessage.GetComponent<MessageTextBase>();
        message.SetTextMsg(text);
        message.SetIsNPC(false);
        StartCoroutine(MessageApplyResize(newMessage));
        _lastMessage = message.Message;
        ChangePreview(text);
    }
    public void ChangePreview(string text)
    {
        string previewText = "";
        if (text.Length > 15)
        {
            for (int i = 0; i < 12; i++)
            {
                previewText += text[i];
            }
            previewText += "...";
        }
        else previewText = text;
        _preview.text = previewText;
    }
    IEnumerator MessageApplyResize(GameObject newMessage)
    {
        yield return new WaitForSeconds(.01f);
        newMessage.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
        newMessage.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
        newMessage.GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
        yield return null;
    }
}
