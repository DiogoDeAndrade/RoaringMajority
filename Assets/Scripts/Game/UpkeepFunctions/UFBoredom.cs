using System;
using System.Collections.Generic;
using UC;
using UnityEngine;

[Serializable]
[PolymorphicName("Boredom")]
public class UFBoredom : UpkeepFunction
{
    public Stat     stat;
    public float    normalValue = 0.15f;
    public float    maxInactivityTicks = 1.0f;
    public float    inactivityBaseValue = 0.15f;
    public float    inactivityScaleValue = 0.0f;

    public override void RunUpkeep(Dictionary<Stat, float> deltaStat, IUpkeepProvider mainObject)
    {
        float deltaValue = GetValue(mainObject);

        if (!deltaStat.TryGetValue(stat, out var value))
        {
            value = deltaValue;
        }
        else
        {
            deltaStat[stat] = value + deltaValue;
        }            
    }

    private float GetValue(IUpkeepProvider mainObject)
    {
        var locationData = mainObject as LocationData;
        if (locationData == null)
        {
            locationData = (mainObject as ProtesterData).location;
        }

        float inactivityPenalty = 0.0f;
        if (locationData != null)
        {
            int inactivityTicks = locationData.inactivityTicks;
            if (inactivityTicks > maxInactivityTicks)
            {
                inactivityPenalty = inactivityBaseValue + inactivityScaleValue * (inactivityTicks - maxInactivityTicks);
            }
        }
        return normalValue + inactivityPenalty;
    }

    public override float GetUpkeep(Stat stat, IUpkeepProvider mainObject)
    {
        if (stat != this.stat) return 0.0f;

        return GetValue(mainObject);
    }
}
