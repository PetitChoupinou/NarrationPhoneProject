using System;
using UnityEngine;

public enum QuestType
{
    Energy,
    Affinity,
    Pub,
    AffinityThreshold,
    Message
}
[Serializable]
public class QuestBase
{
  [SerializeField] private QuestType _type;
    [SerializeField] private string _title;
    [SerializeField] private string _desc;
    [SerializeField] private int _targetValue;
    [SerializeField]private int _currentValue;
    [SerializeField] private int _gemReward=0;
    [SerializeField] private int _energyReward;
    [SerializeField] private bool hasBeenCompleted;

    public string Title { get => _title;}

    public void UpdateValue(int value)
    {
        _currentValue +=value;
        if (_currentValue >= _targetValue&&!hasBeenCompleted)
        {
            Validate();
        }
    }
    public void Validate()
    {
        if (hasBeenCompleted) return;
        if(_gemReward > 0)
        {

        }
        if (_energyReward > 0)
        {
            EnergyManager.Instance.AddEnergy(_energyReward);
        }
        hasBeenCompleted = true;
    }
    public string ToSave()
    {
        string returnString = _title + " " + _currentValue + " " + hasBeenCompleted;
        return returnString;
    }
}
