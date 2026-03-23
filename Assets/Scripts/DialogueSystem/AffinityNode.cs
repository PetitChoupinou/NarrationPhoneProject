using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.MaterialUpgrader;

public class AffinityNode : BaseNode
{
    public float affinityGain;
    public FloatField affinityField;

    public void UpdateAffinityField()
    {
        affinityField.SetValueWithoutNotify(affinityGain);
    }
}

