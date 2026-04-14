using System.Collections;
using TMPro;
using UnityEngine;

abstract public class Notification : MonoBehaviour
{
    [SerializeField] protected TMP_Text _titre;
    [SerializeField] protected TMP_Text _content;
    protected RectTransform _scrollview;
    private Transform _parent;
    [SerializeField] private int _duration;
    public abstract void ButtonPressed();
    public abstract void SetUp(string title,string content,RectTransform scrollview);

    private void Start()
    {
       StartCoroutine( AutoRemove());
        _parent = transform.parent;
    }
    IEnumerator AutoRemove()
    {
        yield return new WaitForSeconds(_duration);
        Destroy(gameObject);
        yield return null;
    }
    private void OnDestroy()
    {
        if (_parent.childCount == 1)
        {
            _scrollview.localScale = Vector3.zero;
        }
    }
}
