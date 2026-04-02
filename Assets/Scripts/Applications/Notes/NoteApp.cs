using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NoteApp : Application
{
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private GameObject _notePrefab;
    [SerializeField] private GameObject _buttonCanvas;
    [SerializeField] private GameObject _headerButton;
    [SerializeField] private TMP_Text _headerText;
    private GameObject _currentNote;

    public GameObject CurrentNote { get => _currentNote; set => _currentNote = value; }

    public override void SetUp(StoryAppSetup setup)
    {
        List<NotesData> notes = setup.Notes;
        for (int i = 0; i < notes.Count; i++)
        {
            string name = notes[i].title;
            string content = notes[i].content;
            AddNote(name, content);
        }
        List<CharacterSheet> characters = setup.Characters;
        for (int i = 0; i < characters.Count; i++)
        {
            string name = characters[i].Name;
            string content = characters[i].BaseNotes;
            AddNote(name, content);
        }
        
    }
    public override void CloseCurrent()
    {
        if (CurrentNote == null) return;
        _currentNote.SetActive(false);
        _headerText.text = "note";
        PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.app);
        _buttonCanvas.SetActive(true);
        _headerButton.SetActive(false);
        _currentNote = null;
    }
    public void AddNote(string name,string content)
    {
        GameObject button = Instantiate(_buttonPrefab, _buttonCanvas.transform);
        GameObject note = Instantiate(_notePrefab, transform);
        button.GetComponent<InAppButton>().SetUp(name, note, _headerButton);
        note.GetComponent<Note>().SetUp(name, content, button, _headerText);
        note.SetActive(false);
    }
}
