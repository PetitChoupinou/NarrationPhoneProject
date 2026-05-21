using System;
using System.Data.SqlTypes;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Photo : MonoBehaviour
{
    private Image _image;
    private Image _imageFull;
    private TMP_Text _previewText;
    private GameObject _fullImagePanel;
    private GameObject _previewImagePanel;
    private GameObject _returnButton;
    private GameObject _parent;
    private DateTime _date;


    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    public void Setup(PhotoData data,TMP_Text preview,GameObject fullImagePanel,GameObject previewImagePanel,Image imageFull,GameObject returnButton,GameObject parent=null)
    {
        if(parent!=null) _parent=parent;
        if(_image == null) _image = GetComponent<Image>();
        _image.sprite = data.image;
        _previewText = preview;
        _date = new DateTime(data.year, data.month, data.day,data.hour,data.minute,0);
        _returnButton = returnButton;
        _fullImagePanel = fullImagePanel;
        _previewImagePanel = previewImagePanel;
        _imageFull = imageFull;
    }
    public void Pressed()
    {
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.deep);
        if (_parent != null) _parent.GetComponent<SpecialAppFolder>().CurrentFile = gameObject;
        string monthString = _date.ToString("MMMM");
        string minutes = "";
        string hours = "";
        if (_date.Minute < 10)
        {
            minutes +="0";
        }
        minutes += _date.Minute;
        if (_date.Hour < 10)
        {
            hours += "0";
        }
        hours += _date.Hour;
        _previewText.text = "le " + _date.Day + " " + monthString + " " + _date.Year + " à " + hours + ":" + minutes;
        _imageFull.sprite = _image.sprite;
        _fullImagePanel.SetActive(true);
        _returnButton.SetActive(true);
        _previewImagePanel.SetActive(false);
    }
}
