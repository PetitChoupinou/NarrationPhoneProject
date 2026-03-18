using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonMsg : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
     public TMP_Text Preview;
     private GameObject _discussion;
     private GameObject _parent;

    public GameObject Parent { get => _parent;}

    public void SetUp(string name,GameObject discussion)
    {
        _parent = transform.parent.gameObject;
        _name.text = name;
        _discussion = discussion;
    }
    public void OnButtonClicked()
    {
        _discussion.SetActive(true);
        _parent.SetActive(false);
    }
}
