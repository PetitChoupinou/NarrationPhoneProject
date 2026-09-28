using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageImage : MonoBehaviour
{
    private HorizontalLayoutGroup _layoutGroup;
    [SerializeField] private Image _image;
    [SerializeField] private float _minWidth = 300;
    [SerializeField] private float _maxWidth = 600;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _layoutGroup = GetComponent<HorizontalLayoutGroup>();
        _layoutGroup.childAlignment = TextAnchor.UpperLeft;
        AppManager appManager = AppManager.Instance;
    }

    public void SetIsNPC(bool isNPC)
    {
        if (isNPC)
        {
            _layoutGroup.childAlignment = TextAnchor.UpperLeft;
        }
        else
        {
            _layoutGroup.childAlignment = TextAnchor.UpperRight;
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
    
}
