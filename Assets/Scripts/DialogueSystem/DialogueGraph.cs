using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "";

    [MenuItem("Window/Dialogue Graph")]
    public static void OpenDialogueGraph()
    {
        DialogueGraph window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void ChangeWindowTitle(string name)
    {
        titleContent = new GUIContent(name);
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

        
        toolbar.Add(new Button(() => Save() ) { text = "Save" });
        toolbar.Add(new Button(() => Save(true) ) { text = "Save as" });
        toolbar.Add(new Button(() => Load() ) { text = "Load" });

        rootVisualElement.Add(toolbar);
    }

    private void Load()
    {
        string path = EditorUtility.OpenFilePanel("Open a file", "Assets/Resources/Dialogues", "asset");
        if(string.IsNullOrEmpty(path))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "File name cannot be empty.", "OK");
            return;
        }

        var graphSaveUtility = GraphSaveUtility.GetInstance(_graphView);
        _fileName = Path.GetFileNameWithoutExtension(path);
        graphSaveUtility.LoadGraph(_fileName);
        ChangeWindowTitle($"Dialogue Graph ({_fileName})");
    }

    private void Save(bool isNew = false)
    {

        if (isNew)
        {
            string path = EditorUtility.SaveFilePanel("Save a file", "Assets/Resources/Dialogues", "New Dialogue", "asset");
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Invalid file name!", "File name cannot be empty.", "OK");
                return;
            }
            _fileName = Path.GetFileNameWithoutExtension(path);
            ChangeWindowTitle($"Dialogue Graph ({_fileName})");
        }
        else
        {
            var existingAsset = Resources.Load(_fileName);
            if (string.IsNullOrEmpty(_fileName) || existingAsset == null)
            {
                EditorUtility.DisplayDialog("Invalid file!", $"The file '{_fileName}' does no exist.", "OK");
                Save(isNew: true);
            }
        }

        var graphSaveUtility = GraphSaveUtility.GetInstance(_graphView);
        graphSaveUtility.SaveGraph(_fileName);
    }

    private void OnEnable()
    {
        ConstructGraph();
        GenerateToolbar();
        GenerateBlackBoard();
    }

    private void GenerateBlackBoard()
    {
        var blackboard = new Blackboard(_graphView)
        {
            name = "Blackboard",
            
        };
        blackboard.Add(new BlackboardSection { title = "Exposed Properties" });
        blackboard.addItemRequested = blackboard =>
        {
            
            Vector2 screenPos = blackboard.LocalToWorld(new Vector2(blackboard.layout.width, 0));
            screenPos = GUIUtility.GUIToScreenPoint(screenPos);
            _graphView.OpenBlackboardTypeWindow(screenPos);
        };
        blackboard.editTextRequested = (blackboard, element, newValue) =>
        {
            var oldPropertyName = ((BlackboardField)element).text;
            if(_graphView.globalPropertiesData.globalProperties.Exists(p => p.Name == newValue))
            {
                EditorUtility.DisplayDialog("Duplicate property name!", $"A property with the name '{newValue}' already exists. Please choose a different name.", "OK");
                return;
            }
            var property = _graphView.FindPropertyByName(oldPropertyName);
            property.Name = newValue;
            ((BlackboardField)element).text = newValue;
        };
        blackboard.SetPosition(new Rect(10, 30, 215, 300));
        _graphView.Add(blackboard);
        _graphView.blackboard = blackboard;
        _graphView.UpdateGlobalVariables();
    }

    

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
