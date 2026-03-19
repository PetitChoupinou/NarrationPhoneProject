using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteApp : MonoBehaviour
{
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private GameObject _notePrefab;
    [SerializeField] private GameObject _buttonCanvas;
    [SerializeField] private GameObject _headerButton;
    [SerializeField] private TMP_Text _headerText;
    private GameObject _currentNote;

    public GameObject CurrentNote { get => _currentNote; set => _currentNote = value; }

    public void SetUp(List<CharacterSheet> characters)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            string name = characters[i].Name;
            string content = characters[i].BaseNotes;
            GameObject button = Instantiate(_buttonPrefab, _buttonCanvas.transform);
            GameObject note = Instantiate(_notePrefab, transform);
            button.GetComponent<InAppButton>().SetUp(name, note, _headerButton);
            note.GetComponent<Note>().SetUp(name, content,button,_headerText);
            note.SetActive(false);

        }
    }
    public void CloseCurrentNote()
    {
        _currentNote.SetActive(false);
        _headerText.text = "note";

    }
}
