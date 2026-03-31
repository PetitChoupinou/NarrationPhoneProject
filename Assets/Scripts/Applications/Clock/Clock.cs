using System;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private TMP_Text _clock;
    [SerializeField] private TMP_Text _date;
    [SerializeField] private TMP_Text _place;
    private string _hours = "";
    private string _minute = "";
    private string _second = "";
    private string _day = "";
    private string _month = "";
    private int _lag;
    public void SetUp()
    {
        TimeSpan timeZone = DateTime.UtcNow.Subtract(DateTime.Now);
        _lag = -timeZone.Hours;
    }
    public void SetUp(int lag,string town)
    {
        TimeSpan timeZone = DateTime.UtcNow.Subtract(DateTime.Now.AddHours(lag));
        _lag = -timeZone.Hours;
        _place.text=town;
    }
    private void Update()
    {
        DateTime thisClock = DateTime.UtcNow.AddHours(_lag);
        if (thisClock.Hour < 10)
        {
            _hours = "0" + thisClock.Hour;
        }
        else _hours =""+ thisClock.Hour;
        if (thisClock.Minute < 10)
        {
            _minute = "0" + thisClock.Minute;
        }
        else _minute = "" + thisClock.Minute;
        if (thisClock.Second < 10)
        {
            _second = "0" + thisClock.Second;
        }
        else _second = "" + thisClock.Second;
        if (thisClock.Day < 10)
        {
            _day = "0" + thisClock.Day;
        }
        else _day = "" + thisClock.Day;
        if (thisClock.Month < 10)
        {
            _month = "0" + thisClock.Month;
        }
        else _month = "" + thisClock.Month;
        _clock.text = _hours + " : " + _minute + " : " + _second;
        _date.text = _day+"/"+_month+"/"+thisClock.Year;
    }
}
