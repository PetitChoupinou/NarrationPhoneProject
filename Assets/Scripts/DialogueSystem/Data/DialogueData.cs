using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[Serializable]
public class DialogueData : ScriptableObject
{
    [SerializeField] float secondsToWait = 60;
    [SerializeField] float secondsToWaitFast = 5;
    [SerializeReference]
    public List<NodeData> nodes = new List<NodeData>();
    [SerializeReference]
    public List<NodeData> temporaryNodes = new List<NodeData>();
    public string entryPointNodeGuid = "";
    [HideInInspector] private bool isLocked;
    [SerializeField] private bool _isLocked;
    [HideInInspector] private bool hasStarted;

    public bool IsLocked { 
        get => isLocked;
        set 
        {
            isLocked = value;
            GetIsActive();
        } 
    }

    public bool HasStarted { 
        get => hasStarted;
        set
        {
            hasStarted = value;
            GetIsActive();
        }
    }

    public event Action<bool> OnDialogueStatutChange;
  

    private void OnValidate()
    {
        isLocked = _isLocked;
    }

    public bool GetBaseIsLocked()
    {
        return _isLocked;
    }

    internal void ResetDialogue()
    {
        isLocked = GetBaseIsLocked();
        foreach (var node in nodes)
        {
            node.isSentCurrent = node.IsSentBase;

        }
    }

    public float GetSecondsToWait()
    {
        //Check option fast wait
        return secondsToWait;
    }

    public bool GetIsActive()
    {
        bool result = !IsLocked && HasStarted;
        //Debug.Log($"{IsLocked} && {HasStarted} ====> {result}");
        OnDialogueStatutChange?.Invoke(result);
        return result;
    }
}
