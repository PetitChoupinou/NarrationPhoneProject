using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class NoteNode : BaseNode
{
    public List<NoteData> noteDatas = new List<NoteData>();

}

[Serializable]
public class NoteData
{
    public NotesData data;

    public NoteData(string title, string content)
    {
        NotesData noteData = new NotesData
        {
            title = title,
            content = content
        };
        this.data = noteData;

    }
}