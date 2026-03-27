using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class SetPropertyNode : BaseNode
{
    public ExposedProperty property;
    private object value;
    public string valueString;
    public VisualElement valueField;
    public DropdownField propertyField;

    public object Value
    {
        get => value;
        set
        {
            this.value = value;
            if (value == null) valueString = "null";
            else valueString = value.ToString();
        }
    }

    public void GetValueFromString()
    {
        if (property == null) return;
        switch (property.type)
        {
            case Type t when t == typeof(int):
                Value = int.Parse(valueString);
                break;
            case Type t when t == typeof(float):
                Value = float.Parse(valueString);
                break;
            case Type t when t == typeof(string):
                Value = valueString;
                break;
            case Type t when t == typeof(bool):
                Value = bool.Parse(valueString);
                break;
        }
        
    }

    public void SetPropertyValue()
    {
        if (property == null) return;
        property.SetValue(Value);
    }

}
