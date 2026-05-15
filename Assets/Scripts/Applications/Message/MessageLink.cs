using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TCG.Core.Dialogues;

public class MessageLink : MonoBehaviour
{
    private HorizontalLayoutGroup _layoutGroup;
    [SerializeField]private Image  _link;
    private ApplicationType _applicationType;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _layoutGroup = GetComponent<HorizontalLayoutGroup>();
        _layoutGroup.childAlignment = TextAnchor.UpperLeft;
    }

    public void SetLinkMsg(ApplicationType type,Sprite spr)
    {
       _applicationType = type;
        _link.sprite = null;
       
    }
    public void OnClickedLink()
    {
        // do la chose à faire
    }
}
