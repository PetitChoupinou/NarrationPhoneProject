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
public class ConditionPropertyNode : BaseNode
{
    public List<Condition> conditions = new List<Condition>();

}

[Serializable]
public class Condition
{
    [SerializeReference]
    public ExposedProperty property;
    public ConditionType condition;
    private object value;
    public string valueString;

        
    public Type typeCondition { get
        {
            if (property == null) return typeof(object);
            return property.type;
        }
    }

    public void GetValueFromString()
    {
        switch (typeCondition.Name)
        {
            case "Int32":
                Value = int.Parse(valueString);
                break;
            case "Single":
                Value = float.Parse(valueString);
                break;
            case "String":
                Value = valueString;
                break;
            case "Boolean":
                Value = bool.Parse(valueString);
                break;
            default:
                Value = null;
                break;
        }
    }

    public object Value { 
        get => value; 
        set 
        { 
            this.value = value;
            if (value == null) valueString = "null";
            else valueString = value.ToString();
        } 
    }

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



    public bool Evaluate(List<ExposedProperty> properties)
    {
        GetValueFromString();
        var foundProperty = properties.FirstOrDefault(prop => prop.Name == property.Name);
        object propertyValue = foundProperty.GetValue();
        int Compare() => ((IComparable)propertyValue).CompareTo(Value);
        switch (condition)
        {
            case ConditionType.Equals:
                return propertyValue.Equals(Value);
            case ConditionType.NotEquals:
                return !propertyValue.Equals(Value);
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