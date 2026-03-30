using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public enum AlarmRepetition
{
    None,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday,
    WeekDays,
    WeekEnd,
    AllWeek,
}
public class Alarm : MonoBehaviour
{
    [SerializeField] private TMP_Text _time;
    [SerializeField] private TMP_Text _tag;
    [SerializeField] private TMP_Text _recurrenceTxt;
    private AlarmRepetition _recurrenceSettings;
    private int _hours, _minutes;
    [SerializeField] private Toggle _active;

    public void SetUp(int hours,int mins, AlarmRepetition recurrenceSettings,bool isActive, string tag = "")
    {
        _hours=hours;
        _minutes= mins;
        string hour="";
        string min="";
        if (hours < 10)
        {
            hour = "0" + hours;
        }
        else
        {
            hour+=hours;
        }
        if (mins < 10)
        {
            min = "0" + mins;
        }
        else
        {
            min += mins;
        }
        _time.text = hour + ":" + min;
        _tag.text = tag;
        _recurrenceSettings = recurrenceSettings;
        
        _active.isOn = isActive;
        UpdateRepText();
    }
    private void UpdateRepText()
    {
        DateTime  now = DateTime.Now;
        DateTime alarm = new DateTime(now.Year, now.Month, now.Day, _hours, _minutes, 0);
        switch (_recurrenceSettings)
        {
            case AlarmRepetition.None:

               
                if (_active.isOn)
                {
                    if (now.Hour > _hours || (now.Minute > _minutes && now.Hour == _hours))
                    {
                        alarm = alarm.AddDays(1);
                    }
                    TimeSpan timeUntilN = alarm.Subtract(now);
                    _recurrenceTxt.text = TextTimeTillAlarm(timeUntilN);
                }
                else _recurrenceTxt.text = "Une fois";
                break;
            case AlarmRepetition.Monday:
                _recurrenceTxt.text = "Lundi ";
               
                if (_active.isOn)
                {
                    if (now.DayOfWeek != DayOfWeek.Monday)
                    {
                        int mod = (1 - (int)now.DayOfWeek) % 7;
                        alarm=alarm.AddDays(mod);
                    }
                    TimeSpan timeUntilM = alarm.Subtract(now);
                    _recurrenceTxt.text +=" | "+ TextTimeTillAlarm(timeUntilM);
                }
                break;
            case AlarmRepetition.Tuesday:
                _recurrenceTxt.text = "Mardi ";
               
                if (_active.isOn)
                {
                    if (now.DayOfWeek != DayOfWeek.Tuesday)
                    {
                        int mod = (2 - (int)now.DayOfWeek) % 7;
                        alarm=alarm.AddDays(mod);
                    }
                    TimeSpan timeUntilTue = alarm.Subtract(now);
                    _recurrenceTxt.text += " | " + TextTimeTillAlarm(timeUntilTue);
                }
                break;
            case AlarmRepetition.Wednesday:
                _recurrenceTxt.text = "Mrecredi ";
              
                if (_active.isOn)
                {
                    if (now.DayOfWeek != DayOfWeek.Wednesday)
                    {
                        int mod = (3 - (int)now.DayOfWeek) % 7;
                        alarm=alarm.AddDays(mod);
                    }
                    TimeSpan timeUntilW = alarm.Subtract(now);
                    _recurrenceTxt.text += " | " + TextTimeTillAlarm(timeUntilW);
                }
                break;
            case AlarmRepetition.Thursday:
                _recurrenceTxt.text = "Jeudi ";              
                if (_active.isOn)
                {
                    if (now.DayOfWeek != DayOfWeek.Thursday)
                    {
                        int mod = (4 - (int)now.DayOfWeek) % 7;
                        alarm=alarm.AddDays(mod);
                    }
                    TimeSpan timeUntilThur = alarm.Subtract(now);
                    _recurrenceTxt.text += " | " + TextTimeTillAlarm(timeUntilThur);
                }
                break;
            case AlarmRepetition.Friday:
                _recurrenceTxt.text = "Vendredi ";
                if (_active.isOn)
                {
                    if (now.DayOfWeek != DayOfWeek.Friday)
                    {
                        int mod = (5 - (int)now.DayOfWeek) % 7;
                        alarm=alarm.AddDays(mod);
                    }
                    TimeSpan timeUntilF = alarm.Subtract(now);
                    _recurrenceTxt.text += " | " + TextTimeTillAlarm(timeUntilF);
                }
                break;
            case AlarmRepetition.Saturday:
                _recurrenceTxt.text = "Samedi ";
                if (_active.isOn)
                {
                    if (now.DayOfWeek != DayOfWeek.Saturday)
                    {
                        int mod = (6 - (int)now.DayOfWeek) % 7;
                        alarm=alarm.AddDays(mod);
                    }
                    TimeSpan timeUntilS = alarm.Subtract(now);
                    _recurrenceTxt.text += " | " + TextTimeTillAlarm(timeUntilS);
                }
                break;
            case AlarmRepetition.Sunday:
                _recurrenceTxt.text = "Dimanche ";
                if (_active.isOn)
                {
                    if (now.DayOfWeek != DayOfWeek.Sunday)
                    {
                        int mod = (0 - (int)now.DayOfWeek) % 7;
                        alarm=alarm.AddDays(mod);
                    }
                    TimeSpan timeUntilSun = alarm.Subtract(now);
                    _recurrenceTxt.text += " | " + TextTimeTillAlarm(timeUntilSun);
                }
                break;
            case AlarmRepetition.WeekDays:
                _recurrenceTxt.text = "Du lundi au vendredi ";

                if (_active.isOn)
                {
                    if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        int mod = (1 - (int)now.DayOfWeek) % 7;
                        alarm=alarm.AddDays(mod);
                    }
                    else if (now.Hour > _hours || (now.Minute > _minutes && now.Hour == _hours) ){
                        alarm=alarm.AddDays(1);
                    }
                    TimeSpan timeUntilWD = alarm.Subtract(now);
                    _recurrenceTxt.text += " | " + TextTimeTillAlarm(timeUntilWD);
                }
                break;
            case AlarmRepetition.WeekEnd:
                _recurrenceTxt.text = "Sam. dim. ";

                if (_active.isOn)
                {
                    if (now.DayOfWeek != DayOfWeek.Saturday && now.DayOfWeek != DayOfWeek.Sunday)
                    {
                        int mod = (6 - (int)now.DayOfWeek) % 7;
                        alarm=alarm.AddDays(mod);
                    }
                    else if (now.Hour > _hours || (now.Minute > _minutes && now.Hour == _hours)) {
                        alarm=alarm.AddDays(1);
                    }
                    TimeSpan timeUntilWE = alarm.Subtract(now);
                    _recurrenceTxt.text += " | " + TextTimeTillAlarm(timeUntilWE);
                }
                break;
            case AlarmRepetition.AllWeek:
                _recurrenceTxt.text = "Quotidiennement ";
                if (_active.isOn)
                {
                    if (now.Hour > _hours || (now.Minute > _minutes && now.Hour == _hours))
                    {
                        alarm=alarm.AddDays(1);
                    }
                    TimeSpan timeUntilWE = alarm.Subtract(now);
                    _recurrenceTxt.text += " | " + TextTimeTillAlarm(timeUntilWE);
                }
                break;
        }
        string recurrenceTxtTemp="";
        if (_recurrenceTxt.text.Length > 50)
        {
            for (int i = 0; i < 47; i++)
            {
                recurrenceTxtTemp += _recurrenceTxt.text[i];
            }
            recurrenceTxtTemp += "...";
            _recurrenceTxt.text=recurrenceTxtTemp;
        }
    }
    private string TextTimeTillAlarm(TimeSpan time)
    {
        string returnValue = "Alarme dans ";
        if (time.Days > 0)
        {
            returnValue += time.Days + " jour";
            if (time.Days >1)
            {
                returnValue += "s";
            }
        }
        if (time.Hours > 0)
        {
            returnValue +=" "+ time.Hours + " heure";
            if (time.Hours > 1)
            {
                returnValue += "s";
            }
        }
        if (time.Minutes > 0)
        {
            returnValue += " " + (time.Minutes+1) + " minute";
            if (time.Minutes > 1)
            {
                returnValue += "s";
            }
        }
        return returnValue;
    }
    private void FixedUpdate()
    {
        if (_active.isOn) UpdateRepText();
    }
    public void Toggle()
    {
        UpdateRepText();
    }
}
