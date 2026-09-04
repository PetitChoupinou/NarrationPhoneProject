using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private int _energyMax;
    private int _currentEnergy;

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
        print(GetNistTime());
    }
}
