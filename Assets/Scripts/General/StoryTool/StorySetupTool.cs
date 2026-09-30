using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
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
    Vector2 _scrollPos2;
    Vector2 _scrollPos3;
    Rect _rect;
    private ActiveWindow _activeWindow=ActiveWindow.Base;
    public GameObject _obj = null;
    public CharacterSheet _chara = null;
    int day = 1;
    int month = 1;
    int year = 1;
    int hour= 0;
    int min = 0;
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
        switch (_activeAppSetup)
        {
            case ApplicationType.Base:
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Name  : " + _activeAppSetup.ToString(), new GUIStyle(GUI.skin.label) { fixedWidth = 200, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                _activeStory.Name = GUILayout.TextField(_activeStory.Name, new GUIStyle(GUI.skin.textField) { fixedWidth = 200, fontSize = 14, alignment = TextAnchor.MiddleCenter });
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                GUILayout.Label("Applications  : ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                for (int i = 0; i < _activeStory.Applications.Count; i++)
                {
                    GameObject app = _activeStory.Applications[i];
                    if (app != null && app.GetComponent<BaseApplication>()) _obj = app;
                    else _obj = null;
                    _obj = (GameObject)EditorGUILayout.ObjectField(_obj, typeof(GameObject), false, GUILayout.MaxWidth(320));
                    if (_obj)
                    {
                        _activeStory.SetApplication(i, _obj);
                    }
                    if (GUILayout.Button("RemoveApp", new GUIStyle(GUI.skin.button) { fixedWidth = 320 }))
                    {
                        _activeStory.RemoveApp(i);
                        return;
                    }
                    else _obj = null;
                }
                if (GUILayout.Button("AddApp", new GUIStyle(GUI.skin.button) { fixedWidth = 320 }))
                {
                    _activeStory.AddApplication();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                GUILayout.Label("Characters  : ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                for (int i = 0; i < _activeStory.Characters.Count; i++)
                {
                    CharacterSheet chara = _activeStory.Characters[i];
                    if (chara != null) _chara = chara;
                    else _chara = null;
                    _chara = (CharacterSheet)EditorGUILayout.ObjectField(_chara, typeof(CharacterSheet), false, GUILayout.MaxWidth(320));
                    if (_chara)
                    {
                        _activeStory.SetChara(i, _chara);
                    }
                    if (GUILayout.Button("RemoveChar", new GUIStyle(GUI.skin.button) { fixedWidth = 320 }))
                    {
                        _activeStory.RemoveChara(i);
                        return;
                    }
                    else _chara = null;
                }
                if (GUILayout.Button("AddChara", new GUIStyle(GUI.skin.button) { fixedWidth = 320 }))
                {
                    _activeStory.AddChara();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                GUILayout.Label("Time  : ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                TimeData timeData = _activeStory.TimeData;
                day = timeData.Day;
                month = timeData.Month;
                year = timeData.Year;
                hour = timeData.Hour;
                min = timeData.Min;
                year = EditorGUILayout.IntField("year :", year, GUILayout.MaxWidth(320));
                month = EditorGUILayout.IntSlider("month :", month, 1, 12, GUILayout.MaxWidth(320));
                int daymax = 31;
                switch (month)
                {
                    case 2:
                        daymax = 28;
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        daymax = 30;
                        break;
                }
                day = EditorGUILayout.IntSlider("day :", day, 1, daymax, GUILayout.MaxWidth(320));
                hour = EditorGUILayout.IntSlider("hour :", hour, 1, 24, GUILayout.MaxWidth(320));
                min = EditorGUILayout.IntSlider("hour :", min, 1, 60, GUILayout.MaxWidth(320));
                _activeStory.SetTime(new TimeData(day, month, year, hour, min));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                GUILayout.Label("SFX  : ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                _scrollPos2 = EditorGUILayout.BeginScrollView(_scrollPos2, GUILayout.MaxHeight(Mathf.Min(600, _activeStory.StorySFX.Count * 86)), GUILayout.MaxWidth(340));
                for (int i = 0; i < _activeStory.StorySFX.Count; i++)
                {
                    SFX sfx = _activeStory.StorySFX[i];
                    sfx.name = GUILayout.TextField(sfx.name, new GUIStyle(GUI.skin.textField) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleCenter });
                    sfx.clip = (AudioClip)EditorGUILayout.ObjectField(sfx.clip, typeof(AudioClip), false, GUILayout.MaxWidth(320));
                    sfx.volume = EditorGUILayout.Slider("volume :", sfx.volume, 0, 1, GUILayout.MaxWidth(320));
                    _activeStory.SetSFX(i, sfx);
                    if (GUILayout.Button("RemoveSFX", new GUIStyle(GUI.skin.button) { fixedWidth = 320 }))
                    {
                        _activeStory.RemoveSFX(i);
                        return;
                    }
                    else _chara = null;
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("AddSFX", new GUIStyle(GUI.skin.button) { fixedWidth = 320 }))
                {
                    _activeStory.AddSFX();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                GUILayout.Label("Music/Amb  : ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                _scrollPos3 = EditorGUILayout.BeginScrollView(_scrollPos3, GUILayout.MaxHeight(Mathf.Min(600, _activeStory.StoryMus.Count * 86)), GUILayout.MaxWidth(340));
                for (int i = 0; i < _activeStory.StoryMus.Count; i++)
                {
                    Music mus = _activeStory.StoryMus[i];
                    mus.name = GUILayout.TextField(mus.name, new GUIStyle(GUI.skin.textField) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleCenter });
                    mus.clip = (AudioClip)EditorGUILayout.ObjectField(mus.clip, typeof(AudioClip), false, GUILayout.MaxWidth(320));
                    mus.volume = EditorGUILayout.Slider("volume :", mus.volume, 0, 1, GUILayout.MaxWidth(320));
                    _activeStory.SetMus(i, mus);
                    if (GUILayout.Button("Remove Music", new GUIStyle(GUI.skin.button) { fixedWidth = 320 }))
                    {
                        _activeStory.RemoveMus(i);
                        return;
                    }
                    else _chara = null;
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Add Music", new GUIStyle(GUI.skin.button) { fixedWidth = 320 }))
                {
                    _activeStory.AddMus();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                break;

            case ApplicationType.Messages:
                EditorGUILayout.BeginVertical();
                _activeStory.MessageBackGround = (Sprite)EditorGUILayout.ObjectField("Background" ,_activeStory.MessageBackGround, typeof(Sprite), false, GUILayout.MaxWidth(200));
                EditorGUILayout.EndVertical();
                break;
        }

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
