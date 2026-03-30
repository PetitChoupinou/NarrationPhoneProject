using UnityEngine;
using UnityEngine.UIElements;

public class AffinityNode : BaseNode
{
    public float affinityGain;
    public FloatField affinityField;

    public void UpdateAffinityField()
    {
        affinityField.SetValueWithoutNotify(affinityGain);
    }
}

