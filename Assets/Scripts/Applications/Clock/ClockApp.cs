using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClockApp : Application
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

    public override void SetUp(List<CharacterSheet> characters)//voir à ce qu'il prennent des clocks?
    {
        GameObject newAlarm = Instantiate(_alarmPrefab, _alarmContent.transform);
        newAlarm.GetComponent<Alarm>().SetUp(14,32,AlarmRepetition.None,true,"aled");
        GameObject newAlarm2 = Instantiate(_alarmPrefab, _alarmContent.transform);
        newAlarm2.GetComponent<Alarm>().SetUp(14, 54, AlarmRepetition.WeekEnd, false);
        _baseClock.SetUp();
        GameObject newClock = Instantiate(_clockPrefab, _clockContent.transform);
        newClock.GetComponent<Clock>().SetUp(+12);
    }
    public void OnActivated()
    {
        _alarmPanel.SetActive(true);
        _clockPanel.SetActive(false);
        _headerTxt.text = "Alarme";
    }
}
