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
    private int _currentEnergy;
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
        var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.google.com");
        var response = myHttpWebRequest.GetResponse();
        string todaysDates = response.Headers["date"];
        return DateTime.ParseExact(todaysDates,
                                   "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                   CultureInfo.InvariantCulture.DateTimeFormat,
                                   DateTimeStyles.AssumeUniversal);
    }
    private void Start()
    {
        _saveManager = SaveManager.instance;
        Load();
    }
    private void OnApplicationQuit()
    {
        Save();
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
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
        _saveManager.Save.SetCurrentEnergy(_energyMax);
        _saveManager.SavePlayerData();
        StopCoroutine(activeEnergyGain);
        activeEnergyGain = null;
    }
    private void Load()
    {
        CurrentEnergy = _saveManager.Save.currentEnergy;
        TimeSpan difference = GetNistTime() - _saveManager.Save.lastAppQuit.CurrentTime;
        int diffInMinute = (int)difference.TotalMinutes;
        AddEnergy(diffInMinute * _energyPerTimeSpan / _timeSpanInMinute);
        activeEnergyGain = StartCoroutine(RecurrentEnergyGain());
    }
    public void AddEnergy (int energyAdded)
    {
        CurrentEnergy += energyAdded;
    }
    public void CostEnergy(int energyRemoved)
    {
        CurrentEnergy -= energyRemoved;
    }
    IEnumerator RecurrentEnergyGain()
    {
        yield return null;
    }
}
