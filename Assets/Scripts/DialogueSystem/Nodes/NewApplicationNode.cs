#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class NewApplicationNode : BaseNode
{
    public ApplicationType applicationType;
    public DropdownField applicationTypeField;

    public void UpdateApplicationChoice(string application)
    {
        
        applicationType = Enum.Parse<ApplicationType>(application);
        applicationTypeField.SetValueWithoutNotify(application);
    }
}
#endif