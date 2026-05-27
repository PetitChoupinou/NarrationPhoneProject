using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class NewFileNode : BaseNode
{
    public string fileName;
    public TextField fileNameField;
    public void UpdateFileName(string name)
    {

        fileName = name;
        fileNameField.SetValueWithoutNotify(fileName);
    }
}
