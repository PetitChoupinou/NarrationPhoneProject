using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.PackageManager.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class GlobalPropertiesData : ScriptableObject
{
    [SerializeReference]
    public List<ExposedProperty> globalProperties = new List<ExposedProperty>();
    
    public ExposedProperty FindProperty(string propertyName)
    {
        var foundProperty = globalProperties.FirstOrDefault(p => p.Name == propertyName);
        return foundProperty;
    }


}



