using System;
using UC;
using UnityEngine;

[Serializable]
[PolymorphicName("Condition")]
public class CStatCondition : ConditionFunction
{
    public enum Comparison
    {
        Equal,
        NotEqual,
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual
    }
    
    public Stat         stat;
    public Comparison   comparisonType;
    public float        referenceValue;

    public override bool Evaluate(IActionProvider mainObject)
    {
        var location = mainObject.GetLocation();

        var value = GameManager.instance.Get(stat, location);
        bool b = false;
        switch (comparisonType)
        {
            case Comparison.Equal:
                b = (value == referenceValue);
                break;
            case Comparison.NotEqual:
                b = (value != referenceValue);
                break;
            case Comparison.Greater:
                b = (value > referenceValue);
                break;
            case Comparison.Less:
                b = (value < referenceValue);
                break;
            case Comparison.GreaterOrEqual:
                b = (value >= referenceValue);
                break;
            case Comparison.LessOrEqual:
                b = (value <= referenceValue);
                break;
            default:
                break;
        }
        return b;
    }
}
