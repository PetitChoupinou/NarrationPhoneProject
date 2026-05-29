using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private TMP_Text _clock;
    [SerializeField] private TMP_Text _date;

    private void OnEnable()
    {
        if(PhoneManager.Instance != null && PhoneManager.Instance.ClockSystem.CurrentTimeData != null)
        {
            UpdateTimeText(PhoneManager.Instance.ClockSystem.CurrentTimeData);
        }
        
        ClockSystem.OnTimeChanged += UpdateTimeText;
    }

    private void OnDisable()
    {
        ClockSystem.OnTimeChanged -= UpdateTimeText;
    }

    private void UpdateTimeText(TimeData currentTimeData)
    {
        string hoursText = "";
        string minutesText = "";
        string dayText = "";
        string monthText = "";

        if (currentTimeData.CurrentTime.Hour < 10)
        {
            hoursText = "0" + currentTimeData.CurrentTime.Hour;
        }
        else hoursText = "" + currentTimeData.CurrentTime.Hour;
        if (currentTimeData.CurrentTime.Minute < 10)
        {
            minutesText = "0" + currentTimeData.CurrentTime.Minute;
        }
        else minutesText = "" + currentTimeData.CurrentTime.Minute;
        if (currentTimeData.CurrentTime.Day < 10)
        {
            dayText = "0" + currentTimeData.CurrentTime.Day;
        }
        else dayText = "" + currentTimeData.CurrentTime.Day;
        if (currentTimeData.CurrentTime.Month < 10)
        {
            monthText = "0" + currentTimeData.CurrentTime.Month;
        }
        else monthText = "" + currentTimeData.CurrentTime.Month;
        _clock.text = hoursText + " : " +  minutesText;
        ;
        if (_date != null)
            _date.text = dayText + "/" + monthText + "/" + currentTimeData.CurrentTime.Year;
    }
}