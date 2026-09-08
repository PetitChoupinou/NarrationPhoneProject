using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueData : ScriptableObject
{
    [SerializeField] float secondsToWait = 60;
    [SerializeField] float secondsToWaitFast = 5;
    [SerializeReference]
    public List<NodeData> nodes = new List<NodeData>();
    public string entryPointNodeGuid = "";
    [HideInInspector] public bool isLocked;
    [SerializeField] private bool _isLocked;
    [HideInInspector] public bool hasStarted;
  

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
}
