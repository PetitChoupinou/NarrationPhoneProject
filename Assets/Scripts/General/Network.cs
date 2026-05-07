using UnityEngine;
using UnityEngine.UI;

public class Network : MonoBehaviour
{
    [SerializeField] Sprite _fullReception;
    [SerializeField] Sprite _midReception;
    [SerializeField] Sprite _badReception;
    private Image _image;
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        _image.sprite = _fullReception; 
    }
    public void ChangeReception(NetworkState networkState)
    {
        switch (networkState)
        {
            case NetworkState.Good:
                _image.sprite = _fullReception;
                break;
            case NetworkState.Mid:
                _image.sprite = _midReception;
                break;
            case NetworkState.Bad:
                _image.sprite = _badReception;
                break;
        }
    }


}
