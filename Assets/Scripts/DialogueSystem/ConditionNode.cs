using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public enum ConditionType
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual
}
public class ConditionNode : BaseNode
{
    public List<Condition> conditions = new List<Condition>();

}

[Serializable]
public class Condition
{
    public ExposedProperty property;
    public ConditionType condition;
    public object value;
    public Type typeCondition;

    Dictionary<string, ConditionType> conditionDic = new Dictionary<string, ConditionType>()
    {
        { "=", ConditionType.Equals },
        { "!=", ConditionType.NotEquals },
        { ">" , ConditionType.GreaterThan },
        {"<" ,ConditionType.LessThan},
        { ">=" , ConditionType.GreaterThanOrEqual},
        { "<=" ,ConditionType.LessThanOrEqual}
    };
    public ConditionType GetConditionType(string conditionText)
    {
        ConditionType type;
        conditionDic.TryGetValue(conditionText, out type);
        return type;


    }

    public string GetConditionText(ConditionType conditionText)
    {
        string text = conditionDic.FirstOrDefault(x => x.Value == conditionText).Key;
        return text;
    }



    public bool Evaluate()
    {
        object propertyValue = property.GetValue();
        int Compare() => ((IComparable)propertyValue).CompareTo(value);
        switch (condition)
        {
            case ConditionType.Equals:
                return propertyValue.Equals(value);
            case ConditionType.NotEquals:
                return !propertyValue.Equals(value);
            case ConditionType.GreaterThan:
                return Compare() > 0;
            case ConditionType.LessThan:
                return Compare() < 0;
            case ConditionType.GreaterThanOrEqual:
                return Compare() >= 0;
            case ConditionType.LessThanOrEqual:
                return Compare() <= 0;
        }
        return false;
    }
}




