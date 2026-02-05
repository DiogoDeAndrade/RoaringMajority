using System;
using UC;
using UnityEngine;

[Serializable]
[PolymorphicName("Constant")]
public class CFConstant : CostFunction
{
    public enum ScaleMode { Constant, ScaleWithCrowd };

    [SerializeField] private Stat       stat;
    [SerializeField] private ScaleMode  scaleMode;
    [SerializeField] private float      value;
    
    public override (Stat stat, float value) GetCost(IActionProvider mainObject)
    {        
        return (stat, GetValue(mainObject));
    }

    float GetValue(IActionProvider mainObject)
    {
        float ret = value;
        switch (scaleMode)
        {
            case ScaleMode.ScaleWithCrowd:
                ret *= GameManager.instance.Get(Globals.statCrowdSize, mainObject.GetLocation());
                break;
        }
        return ret;
    }

    public override bool isStatDelta(IActionProvider mainObject)
    {
        return true;
    }

    public override string GetTooltip(IActionProvider mainObject)
    {
        float v = GetValue(mainObject);
        if (Mathf.Approximately(v, 0.0f)) return null;

        // Check if we can pay it
        var color = stat.color;
        var currentValue = GameManager.instance.Get(stat, mainObject.GetLocation());
        if (currentValue < v)
        {
            color = Color.red;
        }
        v = -v;

        if (v > 0) return $"<color=#{color.ToHex()}>+{Mathf.RoundToInt(v)} {stat.displayName}</color>";
        else return $"<color=#{color.ToHex()}>{Mathf.RoundToInt(v)} {stat.displayName}</color>";
    }
}
