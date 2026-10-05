#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


public class SoundNode : BaseNode
{
    public string soundName;
    public TextField soundNameField;
    public void UpdateSoundField(string newSound)
    {
        soundNameField?.SetValueWithoutNotify(newSound);
    }

}
#endif