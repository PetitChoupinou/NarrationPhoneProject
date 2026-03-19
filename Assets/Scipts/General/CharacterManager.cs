using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<CharacterSheet> _characters;
    [SerializeField] private MessageApp _messageApp;
    [SerializeField] private NoteApp _noteApp;
    void Start()
    {
        _messageApp.SetUp(_characters);
        _noteApp.SetUp(_characters);
    }
}
