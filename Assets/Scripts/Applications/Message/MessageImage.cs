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
    [SerializeField] private GameObject _readMark;
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
            Destroy(_readMark);
            _layoutGroup.childAlignment = TextAnchor.UpperLeft;
            _bgImage.sprite = bgNPC;
        }
        else
        {
            _layoutGroup.childAlignment = TextAnchor.UpperRight;
            _bgImage.sprite = bgPlayer;
            StartCoroutine(SendToRead());
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
    IEnumerator SendToRead()
    {
        _readMark.GetComponent<Image>().sprite = _sent;
        yield return new WaitForSeconds(.5f);
        _readMark.GetComponent<Image>().sprite = _recieved;
        yield return new WaitForSeconds(.5f);
        _readMark.GetComponent<Image>().sprite = _read;
        yield return null;
    }
}
