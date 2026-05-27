using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClockApp : BaseApplication
{
    [SerializeField] private GameObject _alarmPanel;
    [SerializeField] private GameObject _alarmContent;
    [SerializeField] private GameObject _clockContent;
    [SerializeField] private GameObject _clockPanel;
    [SerializeField] private Clock _baseClock;
    [SerializeField] private GameObject _clockPrefab;
    [SerializeField] private GameObject _alarmPrefab;
    [SerializeField] private TMP_Text _headerTxt;
    public override void CloseCurrent()
    {
        return;//Unused in clock
    }

    public override void SetUp(StoryAppSetup setup)//voir à ce qu'il prennent des clocks?
    {
        List<AlarmsData> alarms = setup.Alarms;
        List<ClocksData> clocks = setup.Clocks;
        foreach (AlarmsData alarm in alarms)
        {
            GameObject newAlarm = Instantiate(_alarmPrefab, _alarmContent.transform);
            newAlarm.GetComponent<Alarm>().SetUp(alarm.hours, alarm.minutes, alarm.repetition, alarm.isActive, alarm.tag);
        }
        _baseClock.SetUp();
        foreach(ClocksData clock in clocks)
        {
            GameObject newClock = Instantiate(_clockPrefab, _clockContent.transform);
            newClock.GetComponent<Clock>().SetUp(clock.timeDiff, clock.Town);
        }
    }
    public void OnActivated()
    {
        _alarmPanel.SetActive(true);
        _clockPanel.SetActive(false);
        _headerTxt.text = "Alarme";
    }
}
