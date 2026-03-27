using System;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private TMP_Text _clock;
    [SerializeField] private TMP_Text _date;
    private int _lag;
    public void SetUp()
    {
        TimeSpan timeZone = DateTime.UtcNow.Subtract(DateTime.Now);
        _lag = -timeZone.Hours;
    }
    public void SetUp(int lag)
    {
        TimeSpan timeZone = DateTime.UtcNow.Subtract(DateTime.Now.AddHours(lag));
        _lag = -timeZone.Hours;
    }
    private void Update()
    {
        DateTime thisClock = DateTime.UtcNow.AddHours(_lag);
        string hours = "";
        string minute = "";
        string second = "";
        string day = "";
        string month = "";
        if (thisClock.Hour < 10)
        {
            hours = "0" + thisClock.Hour;
        }
        if (thisClock.Minute < 10)
        {
            minute = "0" + thisClock.Minute;
        }
        if (thisClock.Second < 10)
        {
            second = "0" + thisClock.Second;
        }
        if (thisClock.Day < 10)
        {
            day = "0" + thisClock.Day;
        }
        if (thisClock.Month < 10)
        {
            month = "0" + thisClock.Month;
        }
        _clock.text = thisClock.Hour + " : " + thisClock.Minute + " : " + thisClock.Second;
        _date.text = thisClock.Day+"/"+thisClock.Month+"/"+thisClock.Year;
    }
}
