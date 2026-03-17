using System.Collections;
using UnityEngine;

public class MessageApp : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjectsToDeactivate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CoroutineA());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator CoroutineA()
    {
        yield return new WaitForSeconds(.02f);
        for (int i = 0; i < gameObjectsToDeactivate.Length; i++)
        {
            gameObjectsToDeactivate[i].SetActive(false);
        }
        yield return null;
    }
}
