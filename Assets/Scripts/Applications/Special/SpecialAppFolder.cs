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
    [SerializeField] private GameObject _photoPrefab;
    [SerializeField] private GameObject _filePrefab;
    string _name;

    public void Setup(SpecialFolder folder,Image image,TMP_Text header,GameObject returnButton)
    {
        _displayImage = image;
        _headerTxt = header;
        _name = folder.title;
        _returnButton = returnButton;
        _imagePanel  = image.transform.parent.gameObject;
        foreach (PhotoData photo in folder.spPhoto)
        {
            GameObject photoDisplay =Instantiate(_photoPrefab, transform);
            photoDisplay.GetComponent<Photo>().Setup(photo, _headerTxt, _imagePanel, gameObject, image, _returnButton);
        }
        foreach(NotesData noteData in folder.spNotes)
        {

        }
    }
}
