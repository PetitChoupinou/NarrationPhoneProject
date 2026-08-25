using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ContactApp : BaseApplication
{
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private GameObject _contactPagePrefab;
    [SerializeField] private GameObject _storagePrefab;
    [SerializeField] private GameObject _buttonCanvas;
    private RectTransform _buttonCanvasRect;
    [SerializeField] private GameObject _headerButton;
    [SerializeField] private TMP_Text _headerText;
    private Dictionary<char, GameObject> alphabeticalStorage = new Dictionary<char, GameObject>();
    private GameObject _currentContact;
    private List<ContactPage> _contacts = new List<ContactPage>();

    public GameObject CurrentContact { get => _currentContact; set => _currentContact = value; }

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _buttonCanvasRect = _buttonCanvas.GetComponent<RectTransform>();
        _canvas.enabled = false;

    }
    public void OnActivated()
    {
        _buttonCanvasRect.anchoredPosition = new Vector3(0, -180 - _buttonCanvasRect.sizeDelta.y / 2, 0);
    }
    public override void SetUp(StoryAppSetup setup)
    {
        List<CharacterSheet> characters = setup.Characters;
        for (int i = 0; i < characters.Count; i++)
        {
            string name = characters[i].Name;
            int relation = characters[i].BaseAffinity;
            string num = characters[i].TelNum.numbers;
            Dictionary<CharaEmotion,Sprite> profilePics = characters[i].ProfilePics;
            if (!alphabeticalStorage.ContainsKey(name[0]))
            {
                AlphabeticalStorageCreation(name[0]);
            }
            GameObject button = Instantiate(_buttonPrefab, alphabeticalStorage[name[0]].transform);
            GameObject contact = Instantiate(_contactPagePrefab, transform);
            button.GetComponent<ContactAppButton>().SetUp(name, characters[i].GetBasePicture(),contact, _headerButton);
            var contactPage = contact.GetComponent<ContactPage>();
            contactPage.SetUp(name, num, relation, button, _headerText, profilePics);
            _contacts.Add(contactPage);
            contact.SetActive(false);
        }
        SortStorage();
    }
    public void AlphabeticalStorageCreation(char letter)
    {
        GameObject newStorage = Instantiate(_storagePrefab, _buttonCanvas.transform);
        newStorage.name = "storage "  + letter;
        newStorage.transform.GetChild(0).GetComponent<TMP_Text>().text=""+letter;
        alphabeticalStorage.Add(letter, newStorage);
    }
    private void SortStorage()
    { 
        List<Transform> children = new List<Transform>();
        foreach (Transform child in _buttonCanvas.transform)
            children.Add(child);
        children = children.OrderBy(o => o.name).ToList();

        foreach (Transform child in children)
        {
            child.SetParent(null);
        }

        foreach (Transform child in children)
        {
            child.SetParent(_buttonCanvas.transform); 
        }
    }
    public override void CloseCurrent()
    {
        if (CurrentContact == null) return;
        _currentContact.SetActive(false);
        _headerText.text = "contact";
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.app);
        _buttonCanvas.SetActive(true);
        _headerButton.SetActive(false);
        _currentContact = null;
    }

    public void GainRelation(float value, string targetID)
    {
        var contact = _contacts.FirstOrDefault(x => x.ID == targetID);
        contact.Relation += value;
        Debug.Log($"You gain {value} affinity with {targetID}!");
        Discussion _currentDisc = AppManager.Instance.GetApplication(ApplicationType.Messages).GetComponent<MessageApp>().GetCurrentDiscussion();
        if (_currentDisc == null) return;
        _currentDisc.UpdateRelationhhip(value);
    }

}
