using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HackFolder : MonoBehaviour
{
    Image _displayImage;
    Sprite txtDocSprite;
    TMP_Text _headerTxt;
    GameObject _returnButton;
    GameObject _imagePanel;
    HackApp _hackApp;

    private GameObject _currentFile;

    [SerializeField] private GameObject _photoPrefab;
    [SerializeField] private GameObject _filePrefab;
    [SerializeField] private GameObject _txtDocPrefab;
    [SerializeField] private GameObject _content;
    string _name;
    private PhoneManager _phoneManager;
    private void Awake()
    {
        _phoneManager = PhoneManager.Instance;
    }
    public GameObject CurrentFile { get => _currentFile; set => _currentFile = value; }
    private void OnEnable()
    {
        if (_hackApp == null) return;
        _hackApp.CurrentFolder = gameObject;
        if (_headerTxt)
            _headerTxt.text = _name;
        _phoneManager.ChangeDepth(PhoneManager.AppDepth.inApp);
    }
    public void CloseCurrent()
    {
        if (CurrentFile == null) return;
        if (CurrentFile.GetComponent<Note>())
        {
            CurrentFile.SetActive(false);
            _phoneManager.ChangeDepth(PhoneManager.AppDepth.inApp);
        }
        else if (CurrentFile.GetComponent<Photo>())
        {
            _phoneManager.ChangeDepth(PhoneManager.AppDepth.inApp);
            _imagePanel.SetActive(false);
            gameObject.SetActive(true);          
        }
        CurrentFile = null;
        _content.SetActive(true);
        _headerTxt.text = _name;
    }
    public void Setup(HackFolderSetup folder,Image image,TMP_Text header,GameObject returnButton)
    {
        _hackApp = AppManager.Instance.GetApplication(ApplicationType.Hack).GetComponent<HackApp>();
        _displayImage = image;
        _headerTxt = header;
        _name = folder.title;
        _returnButton = returnButton;
        _imagePanel  = image.transform.parent.gameObject;
        foreach (PhotoData photo in folder.spPhoto)
        {
            GameObject photoDisplay =Instantiate(_photoPrefab, _content.transform);
            photoDisplay.GetComponent<Photo>().Setup(photo, _headerTxt, _imagePanel, gameObject, image, _returnButton,gameObject);
        }
        foreach(NotesData noteData in folder.spNotes)
        {
            GameObject docPreview = Instantiate(_filePrefab, _content.transform);
            GameObject txtDoc = Instantiate(_txtDocPrefab, transform);
            docPreview.GetComponent<InAppButton>().SetUp(noteData.title,txtDoc,_returnButton);
            txtDoc.GetComponent<Note>().SetUp(noteData.title, noteData.title, docPreview,_headerTxt,gameObject);
            txtDoc.GetComponent<Note>().Content.color = Color.black;
            txtDoc.SetActive(false);
        }
    }
}
