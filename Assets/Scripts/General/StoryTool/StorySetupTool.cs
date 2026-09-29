using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class StorySetupTool:EditorWindow
{
    private string FolderPath = "Assets/Resources/StorySetup/";
    private string ShortPath = "StorySetup/";
    private string _extention = ".asset";
    private Vector2 _windowSize;
    private List<StoryAppSetup> _stories=new List<StoryAppSetup>();
    private List<GameObject> _apps=new List<GameObject>();
    private StoryAppSetup _activeStory;
    Vector2 _scrollPos;
    Rect _rect;
    private ActiveWindow _activeWindow=ActiveWindow.Base;
    public GameObject obj = null;
    private ApplicationType _activeAppSetup = ApplicationType.Base;
    enum ActiveWindow
    {
        Base,
        Story
    }

    #region SetUp
    [MenuItem("OneShot/StoryCreator")]
    private static void Init()
    {
        StorySetupTool window = GetWindowWithRect<StorySetupTool>(new Rect(0, 0, 1920, 1080), false);
        window.Show();
    }
    private void OnGUI()
    {
        LoadStories();
        OnGUIUpdate();
    }

    private void LoadStories()
    {
        _stories.Clear();
        _stories.AddRange(Resources.LoadAll<StoryAppSetup>("StorySetup"));

    }
    private void OnGUIUpdate()
    {
        switch (_activeWindow)
        {
            case ActiveWindow.Base:
                BaseDisplay();
                break;
            case ActiveWindow.Story:
                DisplayStory();
                break;
        }
    }
    #endregion
    #region Display
    private void BaseDisplay()
    {
        GUILayout.Label("Stories :", new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        if (_stories.Count > 0)
        {
            _scrollPos=EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MaxHeight(70*_stories.Count));
            for (int i = 0; i < _stories.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.gray;
                if (GUILayout.Button(_stories[i].name, new GUIStyle(GUI.skin.button) { fixedHeight = 60, fontSize = 11, fontStyle = FontStyle.Bold }))
                {
                    _activeStory = _stories[i];
                    _activeWindow = ActiveWindow.Story;
                }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", new GUIStyle(GUI.skin.button) { fixedWidth = 200,fixedHeight = 60, fontSize = 10, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter }))
                {
                    RemoveStory(_stories[i].name);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        GUI.backgroundColor = Color.gray;
        if (GUILayout.Button("Add Room", new GUIStyle(GUI.skin.button) { fixedHeight = 60, fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            CreateNewStory();
        }
    }

    private void DisplayStory()
    {
        GUILayout.Label(_activeStory.Name+ " : "+ _activeAppSetup.ToString(), new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MaxHeight(80));
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("retour", new GUIStyle(GUI.skin.button) { fixedHeight = 60,fixedWidth=60, fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            _activeStory = null;
            _activeWindow = ActiveWindow.Base;
            return;
        }
        GUI.backgroundColor = new Color(0, .53f, .22f);
        if (GUILayout.Button("Save", new GUIStyle(GUI.skin.button) { fixedHeight = 60, fixedWidth = 60, fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            SaveStory();
        }
        GUI.backgroundColor = Color.gray;
        if (GUILayout.Button("Base", new GUIStyle(GUI.skin.button) { fixedHeight = 60,fixedWidth=100, fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            _activeAppSetup = ApplicationType.Base;
        }
        if (_activeStory.Applications.Count > 0)
        {
            foreach (GameObject app in _activeStory.Applications)
            {
                if (GUILayout.Button(app.name, new GUIStyle(GUI.skin.button) { fixedHeight = 60, fixedWidth = 100, fontSize = 11, fontStyle = FontStyle.Bold }))
                {
                    _activeAppSetup = app.GetComponent<BaseApplication>()._appType;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.BeginHorizontal(); 
        GUILayout.Label("Name  : " + _activeAppSetup.ToString(), new GUIStyle(GUI.skin.label) {fixedWidth=200, fontSize = 14, alignment = TextAnchor.MiddleLeft });
        _activeStory.Name = GUILayout.TextField(_activeStory.Name, new GUIStyle(GUI.skin.textField) {fixedWidth=200, fontSize=14,alignment = TextAnchor.MiddleCenter});
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Applications  : " + _activeAppSetup.ToString(), new GUIStyle(GUI.skin.label) { fixedWidth = 200, fontSize = 14, alignment = TextAnchor.MiddleLeft });
        EditorGUILayout.BeginVertical();
        /* List<BaseApplication> apps = new List<BaseApplication>();
         if (_activeStory.Applications.Count > 0)
         {
             foreach (GameObject app in _activeStory.Applications)
             {
                 apps.Add(app.GetComponent<BaseApplication>());
             }
         }
         for (int i = 0; i < _stories.Count; i++)
         {
             apps[i] = (BaseApplication)EditorGUILayout.ObjectField(apps[i], typeof(BaseApplication), false);
         }*/
        foreach (var app in _activeStory.Applications)
        {
            if (app.GetComponent<BaseApplication>() != null)
            {
                _apps.Add(app);
                obj = app;
            }
            obj = (GameObject)EditorGUI.ObjectField(new Rect(3, 300, position.width - 6, 20), "Application : ", obj, typeof(GameObject), false);
            if (obj.GetComponent<BaseApplication>())
            {
                if (GUI.Button(new Rect(3, 325, position.width - 6, 20), "RemoveApp"))
                {
                    _activeStory.RemoveApp(app);
                }
            }
            else obj = null;
        }
        if (GUI.Button(new Rect(3, 325, position.width - 6, 20), "AddApp"))
        {
            _activeStory.AddApplication();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

    }
    #endregion
    private void RemoveStory(string storyName)
    {
        string path = $"{FolderPath}"+storyName+$"{_extention}";
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void CreateNewStory()
    {
        string path = $"{FolderPath}NewStory{_extention}";
        int i = 0;
        while (File.Exists(path))
        {
            i++;
            path = $"{FolderPath}NewStory"+i+ $"{_extention}";
        }
        AssetDatabase.CreateAsset(new StoryAppSetup(), path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void SaveStory()
    {
        string path = $"{FolderPath}" + _activeStory.name + $"{_extention}";
        EditorUtility.SetDirty(_activeStory);
        if (!File.Exists(path))
        {
            Debug.Log("ça marche?");
            AssetDatabase.CreateAsset(_activeStory, path);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
