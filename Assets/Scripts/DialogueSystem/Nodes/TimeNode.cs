using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeNode : BaseNode
{
    public int years;
    public int months;
    public int days;
    public int hours;
    public int minutes;

    public IntegerField yearsField;
    public IntegerField monthsField;
    public IntegerField daysField;
    public IntegerField hoursField;
    public IntegerField minutesField;

    public void UpdateTime(int years, int months, int days, int hours, int minutes)
    {
        yearsField.value = years;
        monthsField.value = months;
        daysField.value = days;
        hoursField.value = hours;
        minutesField.value = minutes;
    }
}
