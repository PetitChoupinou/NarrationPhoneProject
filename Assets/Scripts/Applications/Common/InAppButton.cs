using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InAppButton : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
     public TMP_Text Preview;
     private GameObject _discussion;
     private GameObject _parent;
     private GameObject _returnButton;

    public GameObject Parent { get => _parent;}
    public GameObject Discussion { get => _discussion; set => _discussion = value; }

    public void SetUp(string name,GameObject discussion,GameObject returnButton)
    {
        _parent = transform.parent.gameObject;
        _name.text = name;
        _discussion = discussion;
        _returnButton = returnButton;
    }
    public void OnButtonClicked()
    {
        _discussion.SetActive(true);
        _discussion.GetComponent<RectTransform>().localScale = Vector3.one;
        Discussion d;
        _discussion.TryGetComponent<Discussion>(out d);
        if (d)
        {
            d.Enable();
        }
        _parent.SetActive(false);
        _returnButton.SetActive(true);
    }

    
}
