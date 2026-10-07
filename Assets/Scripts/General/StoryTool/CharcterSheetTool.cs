using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSheetTool : EditorWindow
{
    private string FolderPath = "Assets/Resources/CharacterSheets/";
    private string _extention = ".asset";
    private List<CharacterSheet> _characterSheets = new List<CharacterSheet>();
    private ActiveCharaWindow _activeWindow = ActiveCharaWindow.Base;
    private CharaSetup _charaWindow = CharaSetup.Message;
    private Vector2 _scrollPos;
    private Vector2 _scrollPos2;
    private Vector2 _scrollPos3;
    private Vector2 _scrollPos4;
    private Vector2 _scrollPos5;
    private Vector2 _scrollPos6;
    CharacterSheet _currentSheet;
    private int numValue=0;

    enum CharaSetup
    {
        Message,
        Art,
        Contact,
        Notes
    }

    enum ActiveCharaWindow
    {
        Base,
        Chara
    }

    #region SetUp
    [MenuItem("OneShot/CharacterTool")]
    private static void Init()
    {
        CharacterSheetTool window = GetWindowWithRect<CharacterSheetTool>(new Rect(0, 0, 800, 600), false);
        window.Show();
    }
    private void OnGUI()
    {
        LoadStories();
        OnGUIUpdate();
    }

    private void LoadStories()
    {
        _characterSheets.Clear();
        _characterSheets.AddRange(Resources.LoadAll<CharacterSheet>("CharacterSheets"));

    }
    private void OnGUIUpdate()
    {
        switch (_activeWindow)
        {
            case ActiveCharaWindow.Base:
                BaseDisplay();
                break;
            case ActiveCharaWindow.Chara:
                DisplayChara();
                break;
        }
    }
    #endregion
    #region Display
    private void BaseDisplay()
    {
        LoadStories();
        GUILayout.Label("Characters :", new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        if (_characterSheets.Count > 0)
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MaxHeight(70 * _characterSheets.Count));
            for (int i = 0; i < _characterSheets.Count; i++)
            {
                Debug.Log(_characterSheets[i].Name);
                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.gray;
                if (GUILayout.Button(_characterSheets[i].name, new GUIStyle(GUI.skin.button) { fixedHeight = 60, fontSize = 11, fontStyle = FontStyle.Bold }))
                {
                    Debug.Log(_characterSheets[i].Name);
                    _currentSheet = _characterSheets[i];
                    _charaWindow = CharaSetup.Contact;
                    _activeWindow = ActiveCharaWindow.Chara;
                }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", new GUIStyle(GUI.skin.button) { fixedWidth = 200, fixedHeight = 60, fontSize = 10, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter }))
                {
                    RemoveChara(_characterSheets[i].name);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        GUI.backgroundColor = Color.gray;
        if (GUILayout.Button("Add Character", new GUIStyle(GUI.skin.button) { fixedHeight = 60, fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            CreateNewChara();
        }
    }

    private void DisplayChara()
    {
        GUILayout.Label(_currentSheet.Name + " : " + _charaWindow.ToString(), new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MaxHeight(80));
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("retour", new GUIStyle(GUI.skin.button) { fixedHeight = 60, fixedWidth = 60, fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            _currentSheet = null;
            _activeWindow = ActiveCharaWindow.Base;
            return;
        }
        GUI.backgroundColor = new Color(0, .53f, .22f);
        if (GUILayout.Button("Save", new GUIStyle(GUI.skin.button) { fixedHeight = 60, fixedWidth = 60, fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            SaveChara();
        }
        GUI.backgroundColor = Color.gray;
        if (GUILayout.Button("Contact Page", new GUIStyle(GUI.skin.button) { fixedHeight = 60, fixedWidth = 100, fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            _charaWindow = CharaSetup.Contact;
        }
        if (GUILayout.Button("Messages", new GUIStyle(GUI.skin.button) { fixedHeight = 60, fixedWidth = 100, fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            _charaWindow = CharaSetup.Message;
        }
        if (GUILayout.Button("Art", new GUIStyle(GUI.skin.button) { fixedHeight = 60, fixedWidth = 100, fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            _charaWindow = CharaSetup.Art;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
        switch (_charaWindow)
        {
            case CharaSetup.Contact:
                Debug.Log(_currentSheet.Name);
                _currentSheet.Name = EditorGUILayout.TextField("Name : ", _currentSheet.Name, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(400));
                _currentSheet.BaseAffinity= EditorGUILayout.IntSlider("Base Affinity : ",_currentSheet.BaseAffinity,0,20, GUILayout.MaxWidth(400));
                PhoneNumbers phone = _currentSheet.TelNum;
                phone.title = _currentSheet.Name;
                if (phone.numbers != null && phone.numbers.Length != 0)
                {
                    numValue = Int32.Parse(phone.numbers);
                }
                else numValue = 0;
                numValue = EditorGUILayout.IntField("phone num ", numValue, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(400));
                phone.numbers ="0"+ numValue;
                GUILayout.Label("Call texts ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                _scrollPos2 = EditorGUILayout.BeginScrollView(_scrollPos2, GUILayout.MaxHeight(Mathf.Min(500, phone.callText.Count * 60)), GUILayout.MaxWidth(400));

                for (int i = 0; i < phone.callText.Count; i++)
                {
                    phone.callText[i] = EditorGUILayout.TextField(phone.callText[i], new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(400));
                    if (GUILayout.Button("Remove Text", new GUIStyle(GUI.skin.button) { fixedWidth = 400 }))
                    {
                        phone.callText.RemoveAt(i);
                        break;
                    }
                    if (i != phone.callText.Count - 1)
                        GUILayout.Label(" ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Add Text", new GUIStyle(GUI.skin.button) { fixedWidth =  400}))
                {
                    phone.callText.Add(null);
                }
                phone.ringTime = EditorGUILayout.IntField("ring time ", phone.ringTime, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(400));
                phone.ringAudio = (AudioClip)EditorGUILayout.ObjectField(" ring Audio ", phone.ringAudio, typeof(AudioClip), false, GUILayout.MaxWidth(400));
                phone.callAudio = (AudioClip)EditorGUILayout.ObjectField("call Audio ", phone.callAudio, typeof(AudioClip), false, GUILayout.MaxWidth(400));
                _currentSheet.TelNum = phone;
                int lCount = 0;
                if (_currentSheet.BaseNotes != null && _currentSheet.BaseNotes.Length > 0) 
                    lCount += _currentSheet.BaseNotes.Split('\n').Length - 1;
                _scrollPos3 = EditorGUILayout.BeginScrollView(_scrollPos3, GUILayout.MaxHeight(Mathf.Min(200, lCount*30+60)), GUILayout.MaxWidth(400));
                EditorGUILayout.LabelField("Note :  ", GUILayout.MaxWidth(150));
                _currentSheet.BaseNotes = EditorGUILayout.TextArea(_currentSheet.BaseNotes, new GUIStyle(GUI.skin.textArea) { fontSize = 14, alignment = TextAnchor.MiddleLeft }, GUILayout.MaxWidth(400));
                EditorGUILayout.EndScrollView();
                break;
            case CharaSetup.Message:
                _currentSheet.MessageBackground = (Sprite)EditorGUILayout.ObjectField("Message Background : ", _currentSheet.MessageBackground, typeof(Sprite), false, GUILayout.MaxWidth(200));
                _scrollPos4= EditorGUILayout.BeginScrollView(_scrollPos4, GUILayout.MaxHeight(Mathf.Min(300, _currentSheet.Dialogues.Count * 60)), GUILayout.MaxWidth(400));
                for (int i = 0; i < _currentSheet.Dialogues.Count; i++)
                {
                    _currentSheet.Dialogues[i] = (DialogueData)EditorGUILayout.ObjectField("Dialogues : ", _currentSheet.Dialogues[i], typeof(DialogueData), false, GUILayout.MaxWidth(400));
                    _currentSheet.SetDialogue(i, _currentSheet.Dialogues[i]);
                    if (GUILayout.Button("Remove Dialogue", new GUIStyle(GUI.skin.button) { fixedWidth = 400 }))
                    {
                        _currentSheet.RemoveDialogue(i);
                        break;
                    }
                    if (i != _currentSheet.Dialogues.Count - 1)
                        GUILayout.Label(" ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Add Text", new GUIStyle(GUI.skin.button) { fixedWidth = 400 }))
                {
                    _currentSheet.AddDialogue();
                }
                _currentSheet.dialogueIndex = EditorGUILayout.IntSlider("dialogue index :", _currentSheet.dialogueIndex, 0, _currentSheet.Dialogues.Count-1,GUILayout.MaxWidth(400));
                _scrollPos5 = EditorGUILayout.BeginScrollView(_scrollPos5, GUILayout.MaxHeight(Mathf.Min(300, _currentSheet.WrongPhotoResponses.Count * 40)), GUILayout.MaxWidth(400));
                for (int i = 0; i < _currentSheet.WrongPhotoResponses.Count; i++)
                {
                    _currentSheet.WrongPhotoResponses[i] = EditorGUILayout.TextField(_currentSheet.WrongPhotoResponses[i], new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(400));
                    _currentSheet.SetWPR(i, _currentSheet.WrongPhotoResponses[i]);
                    if (GUILayout.Button("Remove Wrong Photo Response", new GUIStyle(GUI.skin.button) { fixedWidth = 400 }))
                    {
                        _currentSheet.RemoveWPR(i);
                        break;
                    }
                    if (i != _currentSheet.WrongPhotoResponses.Count - 1)
                        GUILayout.Label(" ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Add Wrong Photo Response", new GUIStyle(GUI.skin.button) { fixedWidth = 400 }))
                {
                    _currentSheet.AddWPR();
                }
                break;
            case CharaSetup.Art:
                GUILayout.Label("Emotion pictures : ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                _scrollPos6 = GUILayout.BeginScrollView(_scrollPos6, GUILayout.MaxHeight(Mathf.Min(700, _currentSheet.EmotionPIcs.Count * 90)), GUILayout.MaxWidth(400));
                for (int i = 0; i < _currentSheet.EmotionPIcs.Count;i++)
                {
                    _currentSheet.EmotionPIcs[i].Emotion = (CharaEmotion)EditorGUILayout.EnumPopup("Emotion", (CharaEmotion)_currentSheet.EmotionPIcs[i].Emotion, GUILayout.MaxWidth(380));
                    _currentSheet.EmotionPIcs[i].Picture = (Sprite)EditorGUILayout.ObjectField("Picture : ", (Sprite)_currentSheet.EmotionPIcs[i].Picture, typeof(Sprite), false, GUILayout.MaxWidth(380));
                   if (GUILayout.Button("Remove Emotion", new GUIStyle(GUI.skin.button) { fixedWidth = 380 }))
                    {
                        _currentSheet.EmotionPIcs.RemoveAt(i);
                        break;
                    }
                }
                GUILayout.EndScrollView();
                if (GUILayout.Button("Add Emotion", new GUIStyle(GUI.skin.button) { fixedWidth = 400 }))
                {
                    _currentSheet.EmotionPIcs.Add(null);
                    break;
                }
                break;
        }
    }
    #endregion
    private void RemoveChara(string charaName)
    {
        string path = $"{FolderPath}" + charaName + $"{_extention}";
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void CreateNewChara()
    {
        string path = $"{FolderPath}NewChara{_extention}";
        int i = 0;
        while (File.Exists(path))
        {
            i++;
            path = $"{FolderPath}NewChara" + i + $"{_extention}";
        }
        AssetDatabase.CreateAsset(new CharacterSheet(), path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void SaveChara()
    {
        string path = $"{FolderPath}" + _currentSheet.name + $"{_extention}";
        EditorUtility.SetDirty(_currentSheet);
        if (!File.Exists(path))
        {
            Debug.Log("ça marche?");
            AssetDatabase.CreateAsset(_currentSheet, path);
        }
        AssetDatabase.SaveAssets();
        if (_currentSheet.name != _currentSheet.Name)
        {
            RenameChara(_currentSheet.Name);
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
    }
    private void RenameChara(string newName)
    {
        string path = $"{FolderPath}" + _currentSheet.name + $"{_extention}";
        AssetDatabase.RenameAsset(path, newName);
    }

}
