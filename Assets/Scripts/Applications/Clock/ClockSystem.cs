using System;
using TMPro;
using UnityEngine;

[Serializable]
public class TimeData
{
    [SerializeField, Range(1, 31)]
    private int _day;
    [SerializeField, Range(1, 12)]
    private int _month;
    [SerializeField]
    private int _year;
    [SerializeField, Range(0, 23)]
    private int _hour;
    [SerializeField, Range(0, 59)]
    private int _minute;

    public DateTime CurrentTime;

    public void SetCurrentTime()
    {
        CurrentTime = new DateTime(_year, _month, _day, _hour, _minute, 0);
    }

    public void AddTime(int minutes, int hours, int days, int months, int years)
    {
        SetCurrentTime(); //au cas où
        CurrentTime = CurrentTime.AddYears(years);
        CurrentTime = CurrentTime = CurrentTime.AddMonths(months);
        CurrentTime = CurrentTime.AddDays(days);
        CurrentTime = CurrentTime.AddHours(hours);
        CurrentTime = CurrentTime.AddMinutes(minutes);
        SetTimeFromDateTime();
    }



    private void SetTimeFromDateTime()
    {
        _year = CurrentTime.Year;
        _month = CurrentTime.Month;
        _day = CurrentTime.Day;
        _hour = CurrentTime.Hour;
        _minute = CurrentTime.Minute;
    }


}
public class ClockSystem : MonoBehaviour
{
    TimeData _currentTimeData;
    public delegate void OnTimeChangedDelegate(TimeData newTime);
    public static event OnTimeChangedDelegate OnTimeChanged;

    public TimeData CurrentTimeData { get => _currentTimeData; private set
        {
            _currentTimeData = value;
            _currentTimeData.SetCurrentTime();
            OnTimeChanged?.Invoke(_currentTimeData);
        } 
    }

    public void SetUp(StoryAppSetup setup)
    {
        CurrentTimeData = setup.TimeData;
        AddTime(0, 0, 0, 0, 1);
    }

    public void AddTime(int years, int months, int days, int hours, int minutes)
    {
        CurrentTimeData.AddTime(minutes, hours, days, months, years);
        OnTimeChanged?.Invoke(_currentTimeData);
    }

   
}
