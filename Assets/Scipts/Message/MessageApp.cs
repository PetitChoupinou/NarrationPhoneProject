using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MessageApp : MonoBehaviour
{
    private List<GameObject> gameObjectsToDeactivate=new List<GameObject>();
   [SerializeField] private GameObject _buttonPrefab;
   [SerializeField] private GameObject _discussionPrefab;
   [SerializeField] private GameObject _buttonCanvas;
 
   public  void SetUp(List<CharacterSheet> characters)
    {
        for(int i=0;i<characters.Count; i++)
        {
            string name = characters[i].Name;
            SentText[] texts = characters[i].BaseText;
            GameObject button = Instantiate(_buttonPrefab, _buttonCanvas.transform);
            GameObject discussion = Instantiate(_discussionPrefab,transform);
            button.GetComponent<ButtonMsg>().SetUp(name, discussion);
            discussion.GetComponent<Discussion>().SetUp(name,texts, button);
            gameObjectsToDeactivate.Add(discussion);
            
        }
        StartCoroutine(StartGame());
    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(.02f);
        for (int i = 0; i < gameObjectsToDeactivate.Count; i++)
        {
            gameObjectsToDeactivate[i].SetActive(false);
        }
        yield return null;
    }
}
