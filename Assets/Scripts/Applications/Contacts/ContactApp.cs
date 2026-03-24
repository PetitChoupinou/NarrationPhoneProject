using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ContactApp : Application
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

    public GameObject CurrentContact { get => _currentContact; set => _currentContact = value; }

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _buttonCanvasRect = _buttonCanvas.GetComponent<RectTransform>();
    }
    public void OnActivated()
    {
        _buttonCanvasRect.anchoredPosition = new Vector3(0, -180 - _buttonCanvasRect.sizeDelta.y / 2, 0);
    }
    public override void SetUp(List<CharacterSheet> characters)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            string name = characters[i].Name;
            int relation = characters[i].BaseLikeness;
            string num = characters[i].TelNum;
            Sprite profilePic =characters[i].ProfilePic;
            if (!alphabeticalStorage.ContainsKey(name[0]))
            {
                AlphabeticalStorageCreation(name[0]);
            }
            GameObject button = Instantiate(_buttonPrefab, alphabeticalStorage[name[0]].transform);
            GameObject contact = Instantiate(_contactPagePrefab, transform);
            print(name);
            button.GetComponent<ContactAppButton>().SetUp(name, profilePic,contact, _headerButton);
            contact.GetComponent<ContactPage>().SetUp(name, num, relation, button, _headerText);
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
            child.parent = null;
        }

        foreach (Transform child in children)
        {
            child.parent = _buttonCanvas.transform;
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
}
