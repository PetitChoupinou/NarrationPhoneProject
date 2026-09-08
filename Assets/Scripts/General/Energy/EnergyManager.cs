using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private int _energyMax=100;
    [SerializeField] private int _energyPerTimeSpan=1;
    [SerializeField] private int _timeSpanInMinute=20;
    Coroutine activeEnergyGain;
    [SerializeField] private int _currentEnergy;
    PlayerSaveData _playerSave;
    private SaveManager _saveManager;
    public int CurrentEnergy { get => _currentEnergy;
        set 
        {
            _currentEnergy = value;
            if (_currentEnergy > _energyMax) _currentEnergy = _energyMax;
            if(_currentEnergy<0)_currentEnergy = 0;
        }
    }
    public static DateTime GetNistTime()
    {
        var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.google.com");
        var response = myHttpWebRequest.GetResponse();
        string todaysDates = response.Headers["date"];
        return DateTime.ParseExact(todaysDates,
                                   "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                   CultureInfo.InvariantCulture.DateTimeFormat,
                                   DateTimeStyles.AssumeUniversal);
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
            _saveManager = SaveManager.instance;
            Load();
        }
        else
        {
            Save();
        }
    }
    private void Save()
    {
        _saveManager.Save.SetLastAppQuit(new TimeData(GetNistTime()));
        _saveManager.Save.SetCurrentEnergy(_currentEnergy);
        print(_saveManager.Save.currentEnergy);
        _saveManager.SavePlayerData();
        if (activeEnergyGain == null) return;
        StopCoroutine(activeEnergyGain);
        activeEnergyGain = null;
    }
    private void Load()
    {
        CurrentEnergy = _saveManager.Save.currentEnergy;
        print(CurrentEnergy);
        TimeSpan difference = GetNistTime() - _saveManager.Save.lastAppQuit.CurrentTime;
        int diffInMinute = (int)difference.TotalMinutes;
        print(_saveManager.Save.lastAppQuit.CurrentTime);
        AddEnergy(diffInMinute * _energyPerTimeSpan / _timeSpanInMinute);
        activeEnergyGain = StartCoroutine(RecurrentEnergyGain(diffInMinute));
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
}
