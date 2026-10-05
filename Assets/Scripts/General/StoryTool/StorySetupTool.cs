using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.Rendering;
using UnityEngine.Windows;
using UnityEngine.WSA;
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
    Vector2 _scrollPos4;
    Vector2 _scrollPos5;
    Vector2 _scrollPos6;
    Vector2 _scrollPos8;
    Vector2 _scrollPos11;
    Vector2 _scrollPos13;
    List<Vector2> _scrollPos7=new List<Vector2>();
    List<Vector2> _scrollPos9=new List<Vector2>();
    List<Vector2> _scrollPos10=new List<Vector2>();
    List<Vector2> _scrollPos12=new List<Vector2>();
    int numValue;
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
                _activeStory.Name = EditorGUILayout.TextField("Name : ",_activeStory.Name, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(400));
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
                        break;
                    }
                    if (i != _activeStory.Applications.Count - 1)
                        GUILayout.Label("----------------- ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
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
                        break;
                    }
                    if (i != _activeStory.Characters.Count - 1)
                        GUILayout.Label("----------------- ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
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
                    sfx.name = EditorGUILayout.TextField("name ",sfx.name, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(340));
                    sfx.clip = (AudioClip)EditorGUILayout.ObjectField(sfx.clip, typeof(AudioClip), false, GUILayout.MaxWidth(320));
                    sfx.volume = EditorGUILayout.Slider("volume :", sfx.volume, 0, 1, GUILayout.MaxWidth(320));
                    _activeStory.SetSFX(i, sfx);
                    if (GUILayout.Button("RemoveSFX", new GUIStyle(GUI.skin.button) { fixedWidth = 320 }))
                    {
                        _activeStory.RemoveSFX(i);
                        break;
                    }
                    if (i != _activeStory.StorySFX.Count - 1)
                        GUILayout.Label("----------------- ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
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
                    mus.name = EditorGUILayout.TextField("name ",mus.name, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter },GUILayout.MaxWidth(340));
                    mus.clip = (AudioClip)EditorGUILayout.ObjectField(mus.clip, typeof(AudioClip), false, GUILayout.MaxWidth(320));
                    mus.volume = EditorGUILayout.Slider("volume :", mus.volume, 0, 1, GUILayout.MaxWidth(320));
                    _activeStory.SetMus(i, mus);
                    if (GUILayout.Button("Remove Music", new GUIStyle(GUI.skin.button) { fixedWidth = 320 }))
                    {
                        _activeStory.RemoveMus(i);
                        break;
                    }
                    if (i != _activeStory.StoryMus.Count - 1)
                        GUILayout.Label("----------------- ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
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
            case ApplicationType.Contacts:
                EditorGUILayout.BeginVertical();
                EndGameContact egc = _activeStory.EndGameContact;
                PhoneNumbers phone = egc.phone;
                phone.title= EditorGUILayout.TextField("name ",phone.title, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(960));
                if (phone.numbers != null && phone.numbers.Length != 0)
                {
                    numValue = Int32.Parse(phone.numbers);
                }
                else numValue = 0;
                numValue = EditorGUILayout.IntField("phone num ",numValue, new GUIStyle(GUI.skin.textField){ fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(960));
                phone.numbers=numValue.ToString();
                egc.dialogues[0] = (DialogueData)EditorGUILayout.ObjectField("Dialogue Data ",egc.dialogues[0], typeof(DialogueData), false, GUILayout.MaxWidth(960));
                egc.profilePic = (Sprite)EditorGUILayout.ObjectField("Profile picture ", egc.profilePic, typeof(Sprite), false, GUILayout.MaxWidth(200));
                GUILayout.Label("Call texts ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                _scrollPos4 = EditorGUILayout.BeginScrollView(_scrollPos4, GUILayout.MaxHeight(Mathf.Min(500, phone.callText.Count * 45)), GUILayout.MaxWidth(960));

                for (int i = 0; i < phone.callText.Count; i++)
                {
                    phone.callText[i] = EditorGUILayout.TextField(phone.callText[i], new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter },GUILayout.MaxWidth(960));
                    if (GUILayout.Button("Remove Text", new GUIStyle(GUI.skin.button) { fixedWidth = 940 }))
                    {
                        phone.callText.RemoveAt(i);
                        break;
                    }
                    if (i != phone.callText.Count - 1)
                        GUILayout.Label(" ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Add Text", new GUIStyle(GUI.skin.button) { fixedWidth = 960 }))
                {
                    phone.callText.Add(null);
                }
                phone.ringTime = EditorGUILayout.IntField("ring time ", phone.ringTime, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(960));
                phone.ringAudio = (AudioClip)EditorGUILayout.ObjectField(" ing Audio ",phone.ringAudio, typeof(AudioClip), false, GUILayout.MaxWidth(320));
                phone.callAudio = (AudioClip)EditorGUILayout.ObjectField("call Audio ",phone.callAudio, typeof(AudioClip), false, GUILayout.MaxWidth(320));
                EditorGUILayout.EndVertical();
                egc.phone = phone;
                _activeStory.SetEndConv(egc);
                break;
            case ApplicationType.Notes:
                int lCount = 0;
                if(_activeStory.Notes.Count > 0)
                {
                    foreach (NotesData s in _activeStory.Notes)
                    {
                        if (s.content == null || s.content.Length == 0) continue;
                            lCount += s.content.Split('\n').Length - 1;
                    }
                }
                _scrollPos5 = EditorGUILayout.BeginScrollView(_scrollPos5, GUILayout.MaxHeight(Mathf.Min(800, _activeStory.Notes.Count * 60+ lCount * 20)), GUILayout.MaxWidth(960));
                for (int i = 0; i < _activeStory.Notes.Count; i++)
                {
                    NotesData note = _activeStory.Notes[i];
                    note.title = EditorGUILayout.TextField("title ", note.title, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(940));
                    //TextAreaAttribute(int minLines, int maxLines);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("content ", GUILayout.MaxWidth(150));
                    note.content = EditorGUILayout.TextArea( note.content, new GUIStyle(GUI.skin.textArea) { fontSize = 14, alignment = TextAnchor.MiddleLeft}, GUILayout.MaxWidth(785));
                    EditorGUILayout.EndHorizontal();
                    _activeStory.SetNote(i, note);
                    if (GUILayout.Button("Remove Note", new GUIStyle(GUI.skin.button) { fixedWidth = 940 }))
                    {
                        _activeStory.RemoveNote(i);
                        break;
                    }
                    if (i != _activeStory.Notes.Count - 1)
                        GUILayout.Label("----------------- ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Add Note", new GUIStyle(GUI.skin.button) { fixedWidth = 960 }))
                {
                    _activeStory.AddNote();
                }
                break;
            case ApplicationType.Photos:
                GUILayout.BeginVertical();
                GUILayout.Label("Fichiers :", new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft });
                while (_scrollPos7.Count < _activeStory.Photos.Count)
                    _scrollPos7.Add(Vector2.zero);
                _scrollPos6 = EditorGUILayout.BeginScrollView(_scrollPos6, GUILayout.MaxHeight(Mathf.Min(800, _activeStory.Photos.Count * 400)), GUILayout.MaxWidth(960));
                for (int i = 0; i < _activeStory.Photos.Count; i++)
                {
                    PhotoPreviews folder = _activeStory.Photos[i];
                    GUILayout.Label(folder.title, new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
                    folder.title = EditorGUILayout.TextField("name ", folder.title, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(940));
                    GUILayout.BeginHorizontal();
                    folder.locked = EditorGUILayout.Toggle("is locked ", folder.locked, GUILayout.MaxWidth(200));
                    if (folder.locked) folder.password = EditorGUILayout.TextField("password ", folder.password, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(735));
                    GUILayout.EndHorizontal();
                    _scrollPos7[i] = EditorGUILayout.BeginScrollView(_scrollPos7[i], GUILayout.MaxHeight(400),GUILayout.MinHeight(400), GUILayout.MaxWidth(940));
                    for (int j = 0; j < folder.photoDatas.Count; j++)
                    {
                        PhotoData photo = folder.photoDatas[j];
                        photo.image = (Sprite)EditorGUILayout.ObjectField("image ", photo.image, typeof(Sprite), false, GUILayout.MaxWidth(200));
                        photo.year = EditorGUILayout.IntField("year :", year, GUILayout.MaxWidth(920));
                        photo.month = EditorGUILayout.IntSlider("month :", month, 1, 12, GUILayout.MaxWidth(920));
                        int daymax2 = 31;
                        switch (month)
                        {
                            case 2:
                                daymax2 = 28;
                                break;
                            case 4:
                            case 6:
                            case 9:
                            case 11:
                                daymax2 = 30;
                                break;
                        }
                        photo.day = EditorGUILayout.IntSlider("day :", day, 1, daymax2, GUILayout.MaxWidth(920));
                        photo.hour = EditorGUILayout.IntSlider("hour :", hour, 1, 24, GUILayout.MaxWidth(920));
                        photo.minute = EditorGUILayout.IntSlider("hour :", min, 1, 60, GUILayout.MaxWidth(920));
                        folder.photoDatas[j] = photo;
                        if (GUILayout.Button("Remove photo", new GUIStyle(GUI.skin.button) { fixedWidth = 920 }))
                        {
                            folder.photoDatas.RemoveAt(j);
                            break;
                        }
                       if(i != folder.photoDatas.Count - 1)
                        GUILayout.Label(" ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                    }
                    EditorGUILayout.EndScrollView();
                    if (GUILayout.Button("Add Photo", new GUIStyle(GUI.skin.button) { fixedWidth = 940 }))
                    {
                        folder.photoDatas.Add(new PhotoData());
                    }
                    _activeStory.SetPhotoFolder(i, folder);
                    if (GUILayout.Button("Remove Folder", new GUIStyle(GUI.skin.button) { fixedWidth = 960 }))
                    {
                        _activeStory.RemovePhotoFolder(i);
                        _scrollPos7.RemoveAt(i);
                        break;
                    }
                    GUILayout.Space(20);
                    if (i != _activeStory.Photos.Count - 1)
                        GUILayout.Label("----------------- ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Add Folder", new GUIStyle(GUI.skin.button) { fixedWidth = 960 }))
                {
                    _activeStory.AddPhotoFolder();
                    _scrollPos7.Add( Vector2.zero);
                }
                GUILayout.EndVertical();
                break;
            case ApplicationType.Hack:
                HackSetup hack = _activeStory.HackAppSetup;
                hack.title = EditorGUILayout.TextField("app name ", hack.title, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(940));
                GUILayout.Label("Fichiers :", new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft });
                while (_scrollPos9.Count < hack.folders.Count)
                    _scrollPos9.Add(Vector2.zero);
                while (_scrollPos10.Count < hack.folders.Count)
                    _scrollPos10.Add(Vector2.zero);
                _scrollPos8 = EditorGUILayout.BeginScrollView(_scrollPos8, GUILayout.MaxHeight(Mathf.Min(800, hack.folders.Count * 400)), GUILayout.MaxWidth(1900));
                for(int i = 0; i < hack.folders.Count; i++)
                {
                    HackFolderSetup folderSetup = hack.folders[i];
                    folderSetup.title = EditorGUILayout.TextField("folder name ", folderSetup.title, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(940));
                    folderSetup.isHackedFromStart = EditorGUILayout.Toggle("is unlocked ", folderSetup.isHackedFromStart, GUILayout.MaxWidth(200));

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    GUILayout.Label("Photos ", new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft });
                    _scrollPos9[i] = EditorGUILayout.BeginScrollView(_scrollPos9[i], GUILayout.MaxHeight(400), GUILayout.MinHeight(400), GUILayout.MaxWidth(940));
                    for (int j = 0; j < folderSetup.spPhoto.Count; j++)
                    {
                        PhotoData photo = folderSetup.spPhoto[j];
                        photo.image = (Sprite)EditorGUILayout.ObjectField("image ", photo.image, typeof(Sprite), false, GUILayout.MaxWidth(200));
                        photo.year = EditorGUILayout.IntField("year :", year, GUILayout.MaxWidth(920));
                        photo.month = EditorGUILayout.IntSlider("month :", month, 1, 12, GUILayout.MaxWidth(920));
                        int daymax2 = 31;
                        switch (month)
                        {
                            case 2:
                                daymax2 = 28;
                                break;
                            case 4:
                            case 6:
                            case 9:
                            case 11:
                                daymax2 = 30;
                                break;
                        }
                        photo.day = EditorGUILayout.IntSlider("day :", day, 1, daymax2, GUILayout.MaxWidth(920));
                        photo.hour = EditorGUILayout.IntSlider("hour :", hour, 1, 24, GUILayout.MaxWidth(920));
                        photo.minute = EditorGUILayout.IntSlider("hour :", min, 1, 60, GUILayout.MaxWidth(920));
                        folderSetup.spPhoto[j]=photo;
                        if (GUILayout.Button("Remove photo", new GUIStyle(GUI.skin.button) { fixedWidth = 920 }))
                        {
                            folderSetup.spPhoto.RemoveAt(j);
                            break;
                        }
                        if (i != hack.folders[i].spPhoto.Count - 1)
                            GUILayout.Label(" ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                    }
                    EditorGUILayout.EndScrollView();
                    if (GUILayout.Button("Add Photo", new GUIStyle(GUI.skin.button) { fixedWidth = 940 }))
                    {
                        folderSetup.spPhoto.Add(new PhotoData());
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical();
                    int lCount2 = 0;
                    if(folderSetup.spNotes.Count > 0)
                    {
                        foreach (NotesData s in folderSetup.spNotes)
                        {
                            if(s.content!=null)
                            lCount2 += s.content.Split('\n').Length - 1;
                        }
                    }
                    GUILayout.Label("Notes ", new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft });
                    _scrollPos10[i] = EditorGUILayout.BeginScrollView(_scrollPos10[i], GUILayout.MaxHeight(Mathf.Min(400, folderSetup.spNotes.Count * 60 + lCount2 * 20)), GUILayout.MaxWidth(940));
                    for (int j = 0; j < folderSetup.spNotes.Count; j++)
                    {
                        NotesData note = folderSetup.spNotes[j];
                        if (note.title == null) note.title = "";
                        if (note.content == null) note.content = "";
                        note.title = EditorGUILayout.TextField("title ", note.title, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(940));
                        //TextAreaAttribute(int minLines, int maxLines);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("content ", GUILayout.MaxWidth(150));
                        note.content = EditorGUILayout.TextArea(note.content, new GUIStyle(GUI.skin.textArea) { fontSize = 14, alignment = TextAnchor.MiddleLeft }, GUILayout.MaxWidth(785));
                        EditorGUILayout.EndHorizontal();
                        folderSetup.spNotes[j] = note;
                        if (GUILayout.Button("Remove Note", new GUIStyle(GUI.skin.button) { fixedWidth = 920 }))
                        {
                            folderSetup.spNotes.RemoveAt(j);
                            break;
                        }
                        if (i != hack.folders[i].spNotes.Count - 1)
                            GUILayout.Label(" ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                    }
                    EditorGUILayout.EndScrollView();
                    if (GUILayout.Button("Add Note", new GUIStyle(GUI.skin.button) { fixedWidth = 940 }))
                    {
                        folderSetup.spNotes.Add(new NotesData());

                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    hack.folders[i] = folderSetup;
                    if (GUILayout.Button("Remove Folder", new GUIStyle(GUI.skin.button) { fixedWidth = 1900 }))
                    {
                        hack.folders.RemoveAt(i);
                        _scrollPos10.RemoveAt(i);
                        _scrollPos9.RemoveAt(i);
                        break;
                    }
                    if (i != hack.folders.Count - 1)
                        GUILayout.Label("----------------- ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                }
                GUILayout.EndScrollView();
                if (GUILayout.Button("Add Folder", new GUIStyle(GUI.skin.button) { fixedWidth = 1920 }))
                {
                    hack.folders.Add(new HackFolderSetup());
                }
                _activeStory.SetHackApplication(hack);
                break;
            case ApplicationType.Telephone:
                int lCount3 = 0;
                if (_activeStory.PhoneNumbers.Count > 0)
                {
                    foreach (PhoneNumbers s in _activeStory.PhoneNumbers)
                    {
                        lCount3 += s.callText.Count;
                    }
                }
                while (_scrollPos12.Count < _activeStory.PhoneNumbers.Count)
                    _scrollPos12.Add(Vector2.zero);
                _scrollPos11 = EditorGUILayout.BeginScrollView(_scrollPos11, GUILayout.MaxHeight(Mathf.Min(800, _activeStory.PhoneNumbers.Count*185+ lCount3*40)), GUILayout.MaxWidth(960));
                for (int i = 0; i < _activeStory.PhoneNumbers.Count; i++)
                {
                    phone= _activeStory.PhoneNumbers[i];
                    phone.title = EditorGUILayout.TextField("name ", phone.title, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(940));
                    if (phone.numbers != null && phone.numbers.Length != 0) 
                    {
                        numValue = Int32.Parse(phone.numbers);
                    }
                    else numValue= 0;
                    numValue = EditorGUILayout.IntField("phone num ", numValue, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(940));
                    phone.numbers = numValue.ToString();
                    GUILayout.Label("Call texts ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                    Debug.Log(i + " / " + _scrollPos12.Count);
                    _scrollPos12[i] = EditorGUILayout.BeginScrollView(_scrollPos12[i], GUILayout.MaxHeight(Mathf.Min(500, phone.callText.Count * 60)), GUILayout.MaxWidth(940));
                    for (int j = 0; j < phone.callText.Count; j++)
                    {
                        phone.callText[j] = EditorGUILayout.TextField(phone.callText[j], new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(920));
                        if (GUILayout.Button("Remove Text", new GUIStyle(GUI.skin.button) { fixedWidth = 920 }))
                        {
                            phone.callText.RemoveAt(j);
                            break;
                        }
                        if (j != phone.callText.Count - 1)
                            GUILayout.Label(" ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                    }
                    EditorGUILayout.EndScrollView();
                    if (GUILayout.Button("Add Text", new GUIStyle(GUI.skin.button) { fixedWidth = 940 }))
                    {
                        phone.callText.Add(null);
                    }
                    phone.ringTime = EditorGUILayout.IntField("ring time ", phone.ringTime, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(940));
                    phone.ringAudio = (AudioClip)EditorGUILayout.ObjectField(" ring Audio ", phone.ringAudio, typeof(AudioClip), false, GUILayout.MaxWidth(320));
                    phone.callAudio = (AudioClip)EditorGUILayout.ObjectField("call Audio ", phone.callAudio, typeof(AudioClip), false, GUILayout.MaxWidth(320));
                    _activeStory.SetPhoneNbrr(i, phone);
                    if (GUILayout.Button("Remove PhoneNbr", new GUIStyle(GUI.skin.button) { fixedWidth = 940 }))
                    {
                        _activeStory.RemovePhoneNbr(i);
                        _scrollPos12.RemoveAt(i);
                        break;
                    }
                    if(i!= _activeStory.PhoneNumbers.Count-1)
                         GUILayout.Label("----------------- ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Add PhoneNbr", new GUIStyle(GUI.skin.button) { fixedWidth = 960 }))
                {
                    _activeStory.AddPhoneNbr();
                }
                break;
            case ApplicationType.Internet:
                int lCount4 = 0;
                if (_activeStory.Notes.Count > 0)
                {
                    foreach (InternetSearch s in _activeStory.InternetSeraches)
                    {
                        if (s.text == null || s.text.Length == 0) continue;
                        else
                            lCount4 += s.text.Split('\n').Length - 1;
                    }
                }
                _scrollPos13 = EditorGUILayout.BeginScrollView(_scrollPos13, GUILayout.MaxHeight(Mathf.Min(800, _activeStory.InternetSeraches.Count * 80 + lCount4* 10)), GUILayout.MaxWidth(960));
                for (int i = 0; i < _activeStory.InternetSeraches.Count; i++)
                {
                    InternetSearch search = _activeStory.InternetSeraches[i];
                    search.search = EditorGUILayout.TextField("title ", search.search, new GUIStyle(GUI.skin.textField) { fontSize = 14, alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(940));
                    //TextAreaAttribute(int minLines, int maxLines);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("content ", GUILayout.MaxWidth(150));
                    search.text = EditorGUILayout.TextArea(search.text, new GUIStyle(GUI.skin.textArea) { fontSize = 14, alignment = TextAnchor.MiddleLeft }, GUILayout.MaxWidth(785));
                    EditorGUILayout.EndHorizontal();
                    _activeStory.SetSearch(i, search);
                    if (GUILayout.Button("Remove Search", new GUIStyle(GUI.skin.button) { fixedWidth = 940 }))
                    {
                        _activeStory.RemoveSearch(i);
                        break;
                    }
                    if (i != _activeStory.InternetSeraches.Count - 1)
                        GUILayout.Label("----------------- ", new GUIStyle(GUI.skin.label) { fixedWidth = 320, fontSize = 14, alignment = TextAnchor.MiddleLeft });
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Add Search", new GUIStyle(GUI.skin.button) { fixedWidth = 960 }))
                {
                    _activeStory.AddSearch();
                }
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
