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
        float ret = value;
        switch (scaleMode)
        {
            case ScaleMode.ScaleWithCrowd:
                ret *= GameManager.instance.Get(Globals.statCrowdSize, mainObject.GetLocation());
                break;
        }

        return (stat, ret);
    }
}
