using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Header : MonoBehaviour
{
    [SerializeField] private TMP_Text _time;
    [SerializeField] private Network _network;
    [SerializeField] private Battery battery;
    [SerializeField] private Color _bgColor;
    private Image _backGround;
    private void Awake()
    {
        _backGround = GetComponent<Image>();
    }
    public void AppChangedUpdate(bool isBGLight,bool needBG)
    {
        if (isBGLight)
        {
            _time.color = Color.black;
        }
        else
        {
            _time.color = Color.white;
        }
        if (needBG)
        {
            _backGround.color = _bgColor;
        }
        else
        {
            _backGround.color = Vector4.zero;
        }
    }
}
