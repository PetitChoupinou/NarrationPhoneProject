using System.Collections;
using TMPro;
using UnityEngine;

abstract public class Notification : MonoBehaviour
{
    [SerializeField] protected TMP_Text Titre;
    [SerializeField] protected TMP_Text Content;
    [SerializeField] private int duration;
    public abstract void ButtonPressed();
    public abstract void SetUp(string title,string content);

    private void Start()
    {
       StartCoroutine( AutoRemove());
    }
    IEnumerator AutoRemove()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
        yield return null;
    }
}
