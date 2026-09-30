using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
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
    private StoryAppSetup _activeStory;
    Vector2 _scrollPos;
    Rect _rect;
    private ActiveWindow _activeWindow=ActiveWindow.Base;
    public GameObject _obj = null;
    public CharacterSheet _chara = null;
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
                if (app == null) continue;
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
        GUILayout.Label("Applications  : ", new GUIStyle(GUI.skin.label) { fixedWidth = 200, fontSize = 14, alignment = TextAnchor.MiddleLeft });
        EditorGUILayout.BeginVertical();
        for (int i=0; i<_activeStory.Applications.Count;i++) 
        {
            GameObject app = _activeStory.Applications[i];
            if (app != null && app.GetComponent<BaseApplication>()) _obj = app;
            else _obj = null;
            _obj = (GameObject)EditorGUI.ObjectField(new Rect(3, 180 + i * 50, position.width/6 - 6, 20), "", _obj, typeof(GameObject), false);
            if (_obj)
            {
                _activeStory.SetApplication(i, _obj);
            }
            if (GUI.Button(new Rect(3, 205 + i * 50, position.width/6 - 6, 20), "RemoveApp"))
            {
                _activeStory.RemoveApp(i);
                return;
            }
            else _obj = null;
        }
        if (GUI.Button(new Rect(3, 180+_activeStory.Applications.Count*50, position.width/6 - 6, 20), "AddApp"))
        {
            _activeStory.AddApplication();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Characters  : ", new GUIStyle(GUI.skin.label) { fixedWidth = 200, fontSize = 14, alignment = TextAnchor.MiddleLeft });
        for (int i = 0; i < _activeStory.Characters.Count; i++)
        {
            CharacterSheet chara = _activeStory.Characters[i];
            if (chara != null) _chara = chara;
            else _chara = null;
            _chara = (CharacterSheet)EditorGUI.ObjectField(new Rect(3+position.width/6, 180 + i * 50, position.width / 6 - 6, 20), "", _chara, typeof(CharacterSheet), false);
            if (_chara)
            {
                _activeStory.SetChara(i, _chara);
            }
            if (GUI.Button(new Rect(3 + position.width / 6, 205 + i * 50, position.width / 6 - 6, 20), "RemoveChar"))
            {
                _activeStory.RemoveChara(i);
                return;
            }
            else _chara = null;
        }
        if (GUI.Button(new Rect(3 + position.width / 6, 180 + _activeStory.Characters.Count * 50, position.width / 6 - 6, 20), "AddChara"))
        {
            _activeStory.AddChara();
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
