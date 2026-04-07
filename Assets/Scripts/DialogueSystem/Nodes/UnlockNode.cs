using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;


public class UnlockNode : BaseNode
{
    public string IDCharacter;
    public string IDDialogue;
    public ObjectField characterField;
    public DropdownField dialogueField;
    string[] guids = AssetDatabase.FindAssets("t:CharacterSheet");
    private CharacterSheet _character;

    public void UpdateCharacterField()
    {
        GetCharacter(IDCharacter);
        characterField.value = _character;

    }
    public void UpdateDialogueField()
    {
        dialogueField.value = IDDialogue;
    }
    private void GetCharacter(string ID)
    {
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            _character = AssetDatabase.LoadAssetAtPath<CharacterSheet>(path);
            if(_character.Name == ID)
            {
                break;
            }
            
        }
    }

}
