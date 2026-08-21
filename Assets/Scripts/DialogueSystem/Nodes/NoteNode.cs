
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
public class NoteNode : BaseNode
{
    public List<NoteData> noteDatas = new List<NoteData>();

}
#endif
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
