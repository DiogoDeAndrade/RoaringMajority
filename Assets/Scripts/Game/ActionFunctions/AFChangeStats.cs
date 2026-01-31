using System.Collections.Generic;
using System;
using UnityEngine;
using UC;

[Serializable]
[PolymorphicName("Change Stats")]
public class AFChangeStats : ActionFunction
{
    public enum ScaleMode { Constant, ScaleWithCrowd };
    [Serializable]
    struct StatEntry
    {
        public ScaleMode    scaleMode;
        public Stat         stat;
        public float        value;
    }
    [SerializeField] private List<StatEntry> statsToChange;

    protected override bool RunActionActual(IActionProvider mainObject)
    {
        var loc = mainObject.GetLocation();
        foreach (var stat in statsToChange)
        {
            float value = stat.value;
            switch (stat.scaleMode)
            {
                case ScaleMode.ScaleWithCrowd:
                    value = value * GameManager.instance.GetCrowdSize(loc);
                    break;
            }

            var newValue = GameManager.instance.Get(stat.stat, loc) + value;
            GameManager.instance.Set(stat.stat, newValue, loc);
        }

        return true;
    }
}
