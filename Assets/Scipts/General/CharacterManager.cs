using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<CharacterSheet> _characters;
    [SerializeField] private MessageApp _messageApp;
    void Start()
    {
        _messageApp.SetUp(_characters);
    }
}
