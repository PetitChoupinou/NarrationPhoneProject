using System;
using System.Collections.Generic;
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

    public ConditionType GetConditionType(string conditionText)
    {
        switch(conditionText)
        {
            case "=":
                return ConditionType.Equals;
            case "!=":
                return ConditionType.NotEquals;
            case ">":
                return ConditionType.GreaterThan;
            case "<":
                return ConditionType.LessThan;
            case ">=":
                return ConditionType.GreaterThanOrEqual;
            case "<=":
                return ConditionType.LessThanOrEqual;
            default:
                throw new ArgumentException("Invalid condition type: " + conditionText);
        }
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
            //Uniquement pour float & int
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




