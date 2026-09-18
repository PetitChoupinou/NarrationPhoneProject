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
    [SerializeField] private bool _hasBeenCompleted;

    public string Title { get => _title;}
    public string Desc { get => _desc; }
    public int Targ { get => _targetValue;}
    public int Current { get => _currentValue;}
    public int EReward { get => _energyReward; }
    public int GReward { get => _gemReward; }
    public bool IsComp { get => _hasBeenCompleted;}

    public void UpdateValue(int value)
    {
        _currentValue +=value;
        if (_currentValue >= _targetValue&&!_hasBeenCompleted)
        {
            Validate();
        }
    }
    public void Validate()
    {
        if (_hasBeenCompleted) return;
        if(_gemReward > 0)
        {

        }
        if (_energyReward > 0)
        {
            EnergyManager.Instance.AddEnergy(_energyReward);
        }
        _hasBeenCompleted = true;
    }
    public string ToSave()
    {
        string returnString = _title + " " + _currentValue + " " + _hasBeenCompleted;
        return returnString;
    }
}
