using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "RM/Stat")]
public class Stat : ScriptableObject
{
    public enum StatType { Local, Global };
    public enum LimitType { None, Min, Max, Range };

    public StatType     type = StatType.Local;
    public string       displayName;
    public Color        color;
    public Sprite       icon;
    [TextArea]
    public string       tooltipText;
    public LimitType    limitType = LimitType.Min;
    [ShowIf(nameof(limitIsFloat))]
    public float        limit;
    [ShowIf(nameof(limitIsRange))]
    public Vector2      range;

    bool limitIsFloat => (limitType == LimitType.Min) || (limitType == LimitType.Max);
    bool limitIsRange => (limitType == LimitType.Range);

    public bool isLocal => type == StatType.Local;

    public float ClampToLimit(float value)
    {
        switch (limitType)
        {
            case LimitType.Min:
                if (value < limit) return limit;
                break;
            case LimitType.Max:
                if (value > limit) return limit;
                break;
            case LimitType.Range:
                value = Mathf.Clamp(value, range.x, range.y);
                break;
        }

        return value;
    }
}
