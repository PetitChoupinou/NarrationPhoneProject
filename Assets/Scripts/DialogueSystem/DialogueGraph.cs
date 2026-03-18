using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "New Dialogue";

    [MenuItem("Window/Dialogue Graph")]
    public static void OpenDialogueGraph()
    {
        DialogueGraph window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void ConstructGraph()
    {
        _graphView = new DialogueGraphView(this)
        {
            name = "Dialogue Graph"

        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => Save() ) { text = "Save" });
        toolbar.Add(new Button(() => Load() ) { text = "Load" });

        rootVisualElement.Add(toolbar);
    }

    private void Load()
    {
        if(string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "File name cannot be empty.", "OK");
            return;
        }

        var graphSaveUtility = GraphSaveUtility.GetInstance(_graphView);

        graphSaveUtility.LoadGraph(_fileName);
    }

    private void Save()
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "File name cannot be empty.", "OK");
            return;
        }

        var graphSaveUtility = GraphSaveUtility.GetInstance(_graphView);

        graphSaveUtility.SaveGraph(_fileName);
    }

    private void OnEnable()
    {
        ConstructGraph();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
