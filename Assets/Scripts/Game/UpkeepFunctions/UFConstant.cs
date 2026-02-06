using System;
using System.Collections.Generic;
using UC;
using UnityEngine;

[Serializable]
[PolymorphicName("Constant")]
public class UFConstant : UpkeepFunction
{
    public Stat     stat;
    public float    constantValue;

    public override void RunUpkeep(Dictionary<Stat, float> deltaStat, IUpkeepProvider mainObject)
    {
        if (deltaStat.TryGetValue(stat, out var value))
        {
            deltaStat[stat] = value + constantValue;
        }
        else
        {
            deltaStat[stat] = constantValue;
        }
    }

    public override float GetUpkeep(Stat stat, IUpkeepProvider mainObject)
    {
        if (stat != this.stat) return 0.0f;

        return constantValue;
    }
}
