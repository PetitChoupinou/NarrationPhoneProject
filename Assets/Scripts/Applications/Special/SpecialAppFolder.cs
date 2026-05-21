using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAppFolder : MonoBehaviour
{
    Image _displayImage;
    Sprite txtDocSprite;
    TMP_Text _headerTxt;
    GameObject _returnButton;
    GameObject _imagePanel;

    private GameObject _currentFile;

    [SerializeField] private GameObject _photoPrefab;
    [SerializeField] private GameObject _filePrefab;
    [SerializeField] private GameObject _txtDocPrefab;
    [SerializeField] private GameObject _content;
    string _name;

    public GameObject CurrentFile { get => _currentFile; set => _currentFile = value; }
    private void OnEnable()
    {
        if (_headerTxt)
            _headerTxt.text = _name;
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
    }
    public void Setup(SpecialFolder folder,Image image,TMP_Text header,GameObject returnButton)
    {
        _displayImage = image;
        _headerTxt = header;
        _name = folder.title;
        _returnButton = returnButton;
        _imagePanel  = image.transform.parent.gameObject;
        foreach (PhotoData photo in folder.spPhoto)
        {
            GameObject photoDisplay =Instantiate(_photoPrefab, _content.transform);
            photoDisplay.GetComponent<Photo>().Setup(photo, _headerTxt, _imagePanel, gameObject, image, _returnButton);
        }
        foreach(NotesData noteData in folder.spNotes)
        {
            GameObject docPreview = Instantiate(_filePrefab, _content.transform);
            GameObject txtDoc = Instantiate(_txtDocPrefab, transform);
            docPreview.GetComponent<InAppButton>().SetUp(noteData.title,txtDoc,_returnButton);
            txtDoc.GetComponent<Note>().SetUp(noteData.title, noteData.title, docPreview,_headerTxt,gameObject);
            txtDoc.SetActive(false);
        }
    }
}
