using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NoteApp : BaseApplication
{
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private GameObject _notePrefab;
    [SerializeField] private GameObject _buttonCanvas;
    [SerializeField] private GameObject _headerButton;
    [SerializeField] private TMP_Text _headerText;
    private List<Note> _notes = new List<Note>();

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
        Note note = _notes.FirstOrDefault(x => x.ID == name);
        if(note != null)
        {
            note.AddNote(content);
        }
        else
        {
            GameObject button = Instantiate(_buttonPrefab, _buttonCanvas.transform);
            GameObject newNote = Instantiate(_notePrefab, transform);
            button.GetComponent<InAppButton>().SetUp(name, newNote, _headerButton);
            newNote.GetComponent<Note>().SetUp(name, content, button, _headerText,gameObject);
            newNote.SetActive(false);
            _notes.Add(newNote.GetComponent<Note>());
        }
        
    }
}
