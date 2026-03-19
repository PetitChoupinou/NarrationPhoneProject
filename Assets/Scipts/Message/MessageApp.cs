using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class MessageApp : MonoBehaviour
{
    private List<GameObject> gameObjectsToDeactivate=new List<GameObject>();
   [SerializeField] private GameObject _buttonPrefab;
   [SerializeField] private GameObject _discussionPrefab;
   [SerializeField] private GameObject _buttonCanvas;
   [SerializeField] private GameObject _headerButton;
   [SerializeField] private TMP_Text _headerText;
    private GameObject _currentConv;

    public GameObject CurrentConv { get => _currentConv; set => _currentConv = value; }

    public  void SetUp(List<CharacterSheet> characters)
    {
        for(int i=0;i<characters.Count; i++)
        {
            string name = characters[i].Name;
            SentText[] texts = characters[i].BaseText;
            GameObject button = Instantiate(_buttonPrefab, _buttonCanvas.transform);
            GameObject discussion = Instantiate(_discussionPrefab,transform);
            button.GetComponent<InAppButton>().SetUp(name, discussion,_headerButton);
            discussion.GetComponent<Discussion>().SetUp(name,texts, button,_headerText);
            gameObjectsToDeactivate.Add(discussion);
            
        }
        StartCoroutine(StartGame());
    }
    public void CloseCurrentConv()
    {
        _currentConv.SetActive(false);
        _headerText.text = "message";

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
