using System;
using UC;
using UnityEngine;

[Serializable]
[PolymorphicName("Crowd Size")]
public class CCrowdSize : CComparisonCondition
{
    public override float GetValue(IActionProvider mainObject)
    {
        var location = mainObject.GetLocation();

        return GameManager.instance.GetCrowdSize(location);
    }

    public override string GetTooltip(IActionProvider mainObject)
    {
        if (Evaluate(mainObject))
            return $"<color=#{Globals.statPP.color.ToHex()}>Crowd Size</color> {opString} {referenceValue}";
        else
            return $"<color=#FF0000>Crowd Size {opString} {referenceValue}</color>";
    }
}
