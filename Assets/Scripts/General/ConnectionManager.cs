using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectionManager : MonoBehaviour
{
    private bool _isConnected;
    public static ConnectionManager Instance { get; private set; }

    public bool IsConnected { get => _isConnected;}
    IEnumerator CheckInternetConnection(Action<bool> onResult)
    {
        while (true)
        {
            using (UnityWebRequest www = UnityWebRequest.Head("https://clients3.google.com/generate_204"))
            {
                www.timeout = 5;
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    onResult(true);
                    yield break;
                }
            }
            yield return new WaitForSeconds(30);
            
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
        StartCoroutine(CheckInternetConnection((isConnected) => {
            _isConnected = isConnected;
            QuestManager.Instance.StartQuests();
            EnergyManager.Instance.OfflineEnergyGain();
        }));
    }
}
