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
    public static NotificationManager Instance => instance;
   

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
    }
    public void SendNotifAlarme(string message, string ID)
    {

    }
}


