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


    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    public void Setup(PhotoData data,TMP_Text preview,GameObject fullImagePanel,GameObject previewImagePanel)
    {
        _image.sprite = data.image;
        _previewText = preview;
        string monthString = data.month.ToString("MMMM", CultureInfo.CurrentCulture);
        _previewText.text="le "+data.day + " " + monthString + " " + data.year + " à " + data.hour + " " + data.mintute;
        _fullImagePanel = fullImagePanel;
        _previewImagePanel = previewImagePanel;
        _imageFull = _fullImagePanel.GetComponentInChildren<Image>();
    }
    public void Pressed()
    {
        _imageFull.sprite = _image.sprite;
        _fullImagePanel.SetActive(true);
        _previewImagePanel.SetActive(false);
    }
}
