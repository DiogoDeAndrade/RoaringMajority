using System;
using UC;
using UnityEngine;

[Serializable]
[PolymorphicName("Stat Condition")]
public class CStatCondition : CComparisonCondition
{   
    public Stat         stat;

    public override string GetTooltip(IActionProvider mainObject)
    {
        if (Evaluate(mainObject))
            return $"<color=#{stat.color.ToHex()}>{stat.displayName}</color> {opString} {referenceValue}";
        else
            return $"<color=#FF0000>{stat.displayName} {opString} {referenceValue}</color>";
    }

    public override float GetValue(IActionProvider mainObject)
    {
        var location = mainObject.GetLocation();

        return GameManager.instance.Get(stat, location);
    }
}
