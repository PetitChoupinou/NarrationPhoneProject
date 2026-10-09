using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HackButton : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
     Sprite _sprite;
    TMP_Text _text;

    private TMP_Text _preview;
    private GameObject _parent;
    private GameObject _returnButton;
    private Image _image;
    private GameObject _imagePanel;

    public GameObject Parent { get => _parent; }

    public void SetUp(string name, GameObject returnButton,Image image,PhotoData photoData,GameObject imagePanel,TMP_Text preview)// honnêtement j'aurais pu faire des constructeurs pour quasiment tous mais l'avoir en fonction permet de le faire au moment ou on en a besoin 
    {
        _parent = transform.parent.gameObject;
        _name.text = name;
        _returnButton = returnButton;
        _image = image;
        _sprite = photoData.image;
        _imagePanel = imagePanel;
        _preview = preview;
    }
    public void OnButtonClicked()
    {
        _image.sprite = _sprite;
        _preview.text = _name.text;
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.inApp);
        _parent.SetActive(false);
        _imagePanel.SetActive(true);
        if (_returnButton)
            _returnButton.SetActive(true);
    }
}
