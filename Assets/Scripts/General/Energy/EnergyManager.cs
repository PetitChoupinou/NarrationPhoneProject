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
    public static DateTime GetNistTime()
    {
        //default Windows time server
        const string ntpServer = "time.windows.com";

        // NTP message size - 16 bytes of the digest (RFC 2030)
        var ntpData = new byte[48];

        //Setting the Leap Indicator, Version Number and Mode values
        ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

        var addresses = Dns.GetHostEntry(ntpServer).AddressList;

        //The UDP port number assigned to NTP is 123
        var ipEndPoint = new IPEndPoint(addresses[0], 123);
        //NTP uses UDP

        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            socket.Connect(ipEndPoint);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();
        }

        //Offset to get to the "Transmit Timestamp" field (time at which the reply 
        //departed the server for the client, in 64-bit timestamp format."
        const byte serverReplyTime = 40;

        //Get the seconds part
        ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

        //Get the seconds fraction
        ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

        //Convert From big-endian to little-endian
        intPart = SwapEndianness(intPart);
        fractPart = SwapEndianness(fractPart);

        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

        //**UTC** time
        var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

        return networkDateTime.ToLocalTime();
    }

    // stackoverflow.com/a/3294698/162671
    static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
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
