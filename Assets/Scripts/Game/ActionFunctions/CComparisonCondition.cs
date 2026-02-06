using System;
using UC;
using UnityEngine;

[Serializable]
public abstract class CComparisonCondition : ConditionFunction
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
    
    public Comparison   comparisonType;
    public float        referenceValue;

    protected string opString
    {
        get
        {
            switch (comparisonType)
            {
                case Comparison.Equal:
                    return "==";
                case Comparison.NotEqual:
                    return "!=";
                case Comparison.Greater:
                    return ">";
                case Comparison.Less:
                    return "<";
                case Comparison.GreaterOrEqual:
                    return ">=";
                case Comparison.LessOrEqual:
                    return "<=";
                default:
                    return "";
            }
        }
    }

    public override bool Evaluate(IActionProvider mainObject)
    {
        var location = mainObject.GetLocation();

        var value = GetValue(mainObject);
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

    public abstract float GetValue(IActionProvider mainObject);
}
