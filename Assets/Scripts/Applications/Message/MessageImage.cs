using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MessageImage : MonoBehaviour
{
    private HorizontalLayoutGroup _layoutGroup;
    [SerializeField] private Image _image;
    [SerializeField] private float _minWidth = 300;
    [SerializeField] private float _maxWidth = 600;
    [SerializeField] private Image _bgImage;
    [SerializeField] private Image _readMark;
    [SerializeField] private Sprite _sent;
    [SerializeField] private Sprite _recieved;
    [SerializeField] private Sprite _read;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _layoutGroup = GetComponent<HorizontalLayoutGroup>();
        _layoutGroup.childAlignment = TextAnchor.UpperLeft;
        AppManager appManager = AppManager.Instance;
    }

    public void SetIsNPC(bool isNPC, Sprite bgPlayer, Sprite bgNPC)
    {
        if (isNPC)
        {
            Destroy(_readMark.gameObject);
            _layoutGroup.childAlignment = TextAnchor.LowerLeft;
            _bgImage.sprite = bgNPC;
        }
        else
        {
            _layoutGroup.childAlignment = TextAnchor.LowerRight;
            _bgImage.sprite = bgPlayer;
            Invoke("ChangeMarkToSent", 0);
            Invoke("ChangeMarkToReceived", .5f);
            Invoke("ChangeMarkToRead", 1.5f);
        }
    }

    public void SetImage(Sprite image)
    {
        var spriteWidth = image.rect.width;
        var spriteHeight = image.rect.height;

        var spriteRatio = spriteWidth/spriteHeight;

        var newWidth = Mathf.Clamp(spriteWidth, _minWidth, _maxWidth);
        var newHeight = newWidth / spriteRatio;


        print(_image.rectTransform.sizeDelta);
        _image.rectTransform.sizeDelta = new Vector2(newWidth / _image.rectTransform.rect.width, newHeight / _image.rectTransform.rect.height)*100;
        //_image.preserveAspect = true;
        _image.sprite = image;
        
        
    }
    private void ChangeMarkToSent()
    {
        _readMark.sprite = _sent;
    }
    private void ChangeMarkToReceived()
    {
        _readMark.sprite = _recieved;
    }
    private void ChangeMarkToRead()
    {
        _readMark.sprite = _read;
    }
}
