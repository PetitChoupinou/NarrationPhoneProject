using System;
using UnityEngine;


[Serializable]
public abstract class ExposedProperty
{
    public string Name;
    public Type type { get => GetValue().GetType(); }
    public abstract object GetValue();
    public abstract void SetValue(object value);
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
