using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class NotificationManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static NotificationManager instance = null;
    [SerializeField] private GameObject notifMsgPrefab;
    [SerializeField] private GameObject notifAlarmPrefab;
    [SerializeField] private GameObject notifPanel;
    [SerializeField] private RectTransform notifScrollview;
     private List<AppButton> buttons=new List<AppButton>();
     private Dictionary<string, NotificationMsg> notifs=new Dictionary<string, NotificationMsg>();
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
        print(message);
      
        if (notifs.ContainsKey(ID))
        {
           notifs[ID].ChangeContent(message);
            return;
        }
          GameObject newMsgNotif = Instantiate(notifMsgPrefab, notifPanel.transform);
         notifs.Add(ID, newMsgNotif.GetComponent<NotificationMsg>());
        notifScrollview.localScale = Vector3.one;
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
    public void CheckNotifsOnDestroy(string ID)
    {
        notifs.Remove(ID);
        if (notifs.Count == 0)
        {
            notifScrollview.localScale = Vector3.zero;
        }
    }
}


