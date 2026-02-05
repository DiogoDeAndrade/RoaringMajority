using NaughtyAttributes;
using System;
using UC;
using UnityEngine;

[Serializable]
[PolymorphicName("Add Buff")]
public class EAddBuff : ExecutionFunction
{
    [SerializeField]
    private BuffDef buffDef;

    public override bool Execute(IActionProvider mainObject)
    {
        var loc = mainObject.GetLocation();
        loc.AddBuff(mainObject, buffDef);

        return true;
    }

    public override bool isStatDelta(IActionProvider mainObject)
    {
        return buffDef.isStatDelta(mainObject);
    }

    public override string GetTooltip(IActionProvider mainObject)
    {
        return buffDef.GetTooltip(mainObject);
    }
}
