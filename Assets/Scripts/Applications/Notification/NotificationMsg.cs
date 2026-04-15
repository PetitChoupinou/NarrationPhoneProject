
using System.Collections.Generic;
using UnityEngine;

public class NotificationMsg : Notification
{
    private List<Application> _apps = new List<Application>();
    private MessageApp _messageApp;
    private void Awake()
    {
        _apps = AppManager.Instance.Apps;
        _messageApp = AppManager.Instance.GetApplication(ApplicationType.Messages).GetComponent<MessageApp>();
    }
    public override void ButtonPressed()
    {
        foreach (var app in _apps)
        {
            app.CloseCurrent();
            app.CloseApp();
        }
        _messageApp.GetComponent<Canvas>().enabled = true;
        Discussion discussion = _messageApp.GetDiscussion(_titre.text);
        discussion.MessageButton.GetComponent<InAppButton>().OnButtonClicked();
        Destroy(gameObject);
    }

    public override void SetUp(string title, string content,RectTransform scrollview)
    {
        _scrollview = scrollview;
        if (_scrollview.localScale == Vector3.zero)
        {
            _scrollview.localScale = Vector3.one;
        }
        _titre.text = title;
        _content.text = content;

    }
}
