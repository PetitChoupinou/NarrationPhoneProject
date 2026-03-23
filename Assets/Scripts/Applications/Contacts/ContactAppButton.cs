using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactAppButton : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] private Image _image;

    public TMP_Text Preview;
    private GameObject _discussion;
    private GameObject _parent;
    private GameObject _returnButton;

    public GameObject Parent { get => _parent; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetUp(string name, Sprite image, GameObject discussion, GameObject returnButton)
    {
        _parent = transform.parent.gameObject;
        _name.text = name;
        _discussion = discussion;
        _returnButton = returnButton;
        _image.sprite = image;
    }
    public void OnButtonClicked()
    {
        _discussion.SetActive(true);
        _parent.SetActive(false);
        _returnButton.SetActive(true);
    }
}
