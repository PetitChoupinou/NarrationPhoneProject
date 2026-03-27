using System;
using UnityEngine;


[Serializable]
public abstract class ExposedProperty
{
    public string Name;
    public Type type { get => GetValue().GetType(); }
    public abstract object GetValue();
    public abstract void SetValue(object value);
    public static object GetValueFromString(Type type, string valueString)
    {
        object value = null;
        switch (type)
        {
            case Type t when t == typeof(int):
                value = int.Parse(valueString);
                break;
            case Type t when t == typeof(float):
                value = float.Parse(valueString);
                break;
            case Type t when t == typeof(string):
                value = valueString;
                break;
            case Type t when t == typeof(bool):
                value = bool.Parse(valueString);
                break;
        }
        return value;
    }
}

[Serializable]
public class ExposedProperty<T> : ExposedProperty
{

    public T PropertyValue;

    public ExposedProperty(string propertyName, T value)
    {
        Name = propertyName;
        PropertyValue = value;
    }

    public override object GetValue()
    {
        return PropertyValue;
    }

    public override void SetValue(object value)
    {
        PropertyValue = (T)value;
    }

    
}
