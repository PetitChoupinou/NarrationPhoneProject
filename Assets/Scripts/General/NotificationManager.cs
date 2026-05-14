using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NotificationManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static NotificationManager instance = null;
    [SerializeField] private GameObject notifMsgPrefab;
    [SerializeField] private GameObject notifAlarmPrefab;
    [SerializeField] private GameObject notifPanel;
    [SerializeField] private RectTransform notifScrollview;
     private List<AppButton> buttons=new List<AppButton>();
    public static NotificationManager Instance => instance;

    public List<AppButton> Buttons { get => buttons; set => buttons = value; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    public void SendNotifText(string message, string ID)
    {
        GameObject newMsgNotif = Instantiate(notifMsgPrefab, notifPanel.transform);
        newMsgNotif.GetComponent<NotificationMsg>().SetUp(ID, message, notifScrollview);
        AppButton button = FindButton(ApplicationType.Messages);
        button.SetNotifUp();
    }
    public void SendNotifAlarme(string message, string ID)
    {

    }
    private AppButton FindButton(ApplicationType type)
    {
        return Buttons.Find(x=>x.Type == type);
    }
}


