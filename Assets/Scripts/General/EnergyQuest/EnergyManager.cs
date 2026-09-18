using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private int _energyMax=100;
    [SerializeField] private int _energyPerTimeSpan=1;
    [SerializeField] private int _timeSpanInMinute=20;
    Coroutine activeEnergyGain;
    [SerializeField] private int _currentEnergy;
    PlayerSaveData _playerSave;
    private SaveManager _saveManager;
    public static EnergyManager Instance { get; private set; }
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
    public int CurrentEnergy { get => _currentEnergy;
        set 
        {
            _currentEnergy = value;
            if (_currentEnergy > _energyMax) _currentEnergy = _energyMax;
            if(_currentEnergy<0)_currentEnergy = 0;
        }
    }
   
    private void Start()
    {
   //     _saveManager = SaveManager.instance;
     //   Load();
    }
    private void OnApplicationQuit()
    {
        Save();
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            _saveManager = SaveManager.Instance;
            Load();
        }
        else
        {
            Save();
        }
    }
    private void Save()
    {
        if (ConnectionManager.Instance.IsConnected)
        {
            _saveManager.Save.SetLastAppQuit(new TimeData(InternetConnection.GetNistTime()));
        }
        else
        {
            _saveManager.Save.SetLastAppQuit(new TimeData(DateTime.Now));
        }
        _saveManager.Save.SetCurrentEnergy(_currentEnergy);
        _saveManager.SavePlayerData();
        if (activeEnergyGain == null) return;
        StopCoroutine(activeEnergyGain);
        activeEnergyGain = null;
    }
    private void Load()
    {
        CurrentEnergy = _saveManager.Save.currentEnergy;
        activeEnergyGain = StartCoroutine(RecurrentEnergyGain(0));
    }
    public void AddEnergy (int energyAdded)
    {
        CurrentEnergy += energyAdded;
    }
    public void CostEnergy(int energyRemoved)
    {
        CurrentEnergy -= energyRemoved;
    }
    IEnumerator RecurrentEnergyGain(int diffInMin)
    {
        WaitForSeconds wait = new WaitForSeconds(_timeSpanInMinute*60);
        float timer= diffInMin * _energyPerTimeSpan % _timeSpanInMinute;
        yield return new WaitForSeconds((_timeSpanInMinute - timer)*60);
        while (true)
        {
            AddEnergy(_energyPerTimeSpan);
            yield return wait;
        }
    }
    public void OfflineEnergyGain()
    {
        print(CurrentEnergy);
        TimeSpan difference = InternetConnection.GetNistTime() - _saveManager.Save.lastAppQuit.CurrentTime;
        int diffInMinute = (int)difference.TotalMinutes;
        print(_saveManager.Save.lastAppQuit.CurrentTime);
        AddEnergy(diffInMinute * _energyPerTimeSpan / _timeSpanInMinute);
        _saveManager.SavePlayerData();
    }
}
