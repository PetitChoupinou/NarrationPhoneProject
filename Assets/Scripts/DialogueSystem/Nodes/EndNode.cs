#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;


public class EndNode : BaseNode
{
    public EndID endID;
    public DropdownField endIDField;

    public void UpdateEndIDField(EndID newEndID)
    {
        endID = newEndID;
        endIDField?.SetValueWithoutNotify(endID.ToString());
    }
}
#endif
