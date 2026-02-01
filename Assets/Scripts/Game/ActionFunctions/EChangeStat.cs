using System.Collections.Generic;
using System;
using UnityEngine;
using UC;
using NaughtyAttributes;

[Serializable]
[PolymorphicName("Change Stat")]
public class EChangeStat : ExecutionFunction
{
    public enum ScaleMode { Constant, ScaleWithCrowd };
    
    [SerializeField] 
    private ScaleMode    scaleMode;
    [SerializeField] 
    private Stat         stat;
    [SerializeField] 
    private float        baseValue;
    [SerializeField, ShowIf(nameof(hasScale))] 
    private float        multiplier;

    bool hasScale => scaleMode == ScaleMode.ScaleWithCrowd;

    public override bool Execute(IActionProvider mainObject)
    {
        var loc = mainObject.GetLocation();
        float value = baseValue;
        switch (scaleMode)
        {
            case ScaleMode.ScaleWithCrowd:
                value = baseValue + multiplier * GameManager.instance.Get(Globals.statCrowdSize, loc);
                break;
        }

        var newValue = GameManager.instance.Get(stat, loc) + value;
        GameManager.instance.Set(stat, newValue, loc);

        return true;
    }
}
