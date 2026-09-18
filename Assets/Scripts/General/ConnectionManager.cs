using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectionManager : MonoBehaviour
{
    private bool _isConnected;
    public static ConnectionManager Instance { get; private set; }

    public bool IsConnected { get => _isConnected;}
    IEnumerator checkInternetConnection(Action<bool> action)
    {
        UnityWebRequest www = new UnityWebRequest("http://google.com");
        yield return www;
        if (www.error != null)
        {
            yield return new WaitForSeconds(60);
            StartCoroutine(checkInternetConnection((isConnected) => {
                _isConnected = isConnected;
                QuestManager.Instance.StartQuests();
                EnergyManager.Instance.OfflineEnergyGain();
            }));
        }
        else
        {
            action(true);
        }
    }
     private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        StartCoroutine(checkInternetConnection((isConnected) => {
            _isConnected = isConnected;
            QuestManager.Instance.StartQuests();
            EnergyManager.Instance.OfflineEnergyGain();
        }));
    }
}
