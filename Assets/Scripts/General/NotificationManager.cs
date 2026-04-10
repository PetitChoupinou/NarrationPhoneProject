using UnityEngine;
using UnityEngine.Events;

public class NotificationManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static NotificationManager instance = null;
    [SerializeField] private GameObject notifMsgPrefab;
    [SerializeField] private GameObject notifAlarmPrefab;
    [SerializeField] private GameObject notifPanel;
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
     public void SendNotifText(string message,string ID)
    {

    }
    public void SendNotifAlarme(string message, string ID)
    {

    }
}


