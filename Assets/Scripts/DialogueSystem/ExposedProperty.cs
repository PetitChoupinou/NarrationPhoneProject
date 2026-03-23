using System;
using UnityEngine;


[Serializable]
public abstract class ExposedProperty
{
    public string Name;
    public Type type;
    public abstract object GetValue();
    public abstract void SetValue(object value);
}

[Serializable]
public class ExposedProperty<T> : ExposedProperty
{

    public T PropertyValue;

    public ExposedProperty(string propertyName)
    {
        Name = propertyName;
        type = typeof(T);
        PropertyValue = default(T);
    }

    public override object GetValue()
    {
        return PropertyValue;
    }

    public override void SetValue(object value)
    {
        Debug.Log(typeof(T).ToString());
        PropertyValue = (T)value;
    }
}
