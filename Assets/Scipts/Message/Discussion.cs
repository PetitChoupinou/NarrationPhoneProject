using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class Discussion : MonoBehaviour
{
    private TMP_Text _lastMessage;
    private string _iD;
    private TMP_Text _preview;
    private GameObject _messageButton;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _messagePrefab;

    public string ID { get => _iD;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetUp(string name,SentText[] texts,GameObject button)
    {
        _iD = name;
        _messageButton = button;
        _preview = _messageButton.GetComponent<ButtonMsg>().Preview;
        if (texts.Length<=0) return;
        for (int i = 0; i < texts.Length; i++)
        {
            AddMessage(texts[i].Text, texts[i].isNPC);
        }
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
        StopAllCoroutines();
        GameObject newMessage = Instantiate(_messagePrefab, _content.transform);
        MessageTextBase message = newMessage.GetComponent<MessageTextBase>();
        message.SetTextMsg(text);
        message.SetIsNPC(isNPC);
        _lastMessage = message.Message;
   ChangePreview(text);
        StartCoroutine(MessageApplyResize(newMessage));
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
        newMessage.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
        newMessage.GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
        yield return null;
    }
    public void CloseDiscussion()
    {
        _messageButton.GetComponent<ButtonMsg>().Parent.SetActive(true);
        gameObject.SetActive(false);
    }
}
