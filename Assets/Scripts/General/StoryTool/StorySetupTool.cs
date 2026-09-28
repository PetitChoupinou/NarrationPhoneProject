using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

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
        BaseDisplay();
    }

    private void LoadStories()
    {
        _stories.Clear();
        _stories.AddRange(Resources.LoadAll<StoryAppSetup>("StorySetup"));

    }
    private void OnGUIUpdate()
    {
        BaseDisplay();
        switch (_activeWindow)
        {
            case ActiveWindow.Base:
                break;
            case ActiveWindow.Story:
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
            EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MaxHeight(70*_stories.Count));
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
        string path = $"{FolderPath}Rooms{_extention}";
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
