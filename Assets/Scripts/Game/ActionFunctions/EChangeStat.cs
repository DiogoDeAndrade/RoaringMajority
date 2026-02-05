using NaughtyAttributes;
using System;
using UC;
using UnityEngine;

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
        float value = GetValue(mainObject);
        var newValue = GameManager.instance.Get(stat, loc) + value;
        GameManager.instance.Set(stat, newValue, loc);

        return true;
    }

    public float GetValue(IActionProvider mainObject)
    {
        var loc = mainObject.GetLocation();
        float value = baseValue;
        switch (scaleMode)
        {
            case ScaleMode.ScaleWithCrowd:
                value = baseValue + multiplier * GameManager.instance.Get(Globals.statCrowdSize, loc);
                break;
        }
        return value;
    }

    public override bool isStatDelta(IActionProvider mainObject)
    {
        return true;
    }

    public override string GetTooltip(IActionProvider mainObject)
    {
        float v = GetValue(mainObject);
        if (Mathf.Approximately(v, 0.0f)) return null;

        if (v > 0) return $"<color=#{stat.color.ToHex()}>+{Mathf.RoundToInt(v)} {stat.displayName}</color>";
        else return $"<color=#{stat.color.ToHex()}>{Mathf.RoundToInt(v)} {stat.displayName}</color>";
    }
}
