using NaughtyAttributes;
using UC;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff_StatOverTime", menuName = "RM/Buffs/Stat Over Time")]
public class Buff_StatOverTime : StatefulBuffDef<Buff_StatOverTime.Data>
{
    public class Data : BaseBuffData
    {
        public override bool Run(IActionProvider executor)
        {
            var buffDef = (Buff_StatOverTime)def;

            float   value = buffDef.GetValue(caster);
            var     loc = caster.GetLocation();
            var     newValue = GameManager.instance.Get(buffDef.stat, loc) + value;
            GameManager.instance.Set(buffDef.stat, newValue, loc);

            return true;
        }

        public override string GetTooltip(Stat stat, IActionProvider executor)
        {
            var buffDef = (Buff_StatOverTime)def;
            if (buffDef.stat != stat) return null;

            float   v = buffDef.GetValue(caster);
            string  vString = ((v > 0.0f) ? "+" : "") + $"{v}";

            switch (buffDef._clockType)
            {
                case ClockType.Realtime:
                    return $"{buffDef._displayName}: <color=#{stat.color.ToHex()}>{vString}</color>/{buffDef._tickTime}s, for {duration}s";
                case ClockType.TurnBased:
                    return $"{buffDef._displayName}: <color=#{stat.color.ToHex()}>{vString}</color>/tick, for {Mathf.CeilToInt(duration)} ticks";
            }
            
            throw new System.NotImplementedException();
        }
    }

    public enum ScaleMode { Constant, ScaleWithCrowd, ScaleWithType };

    [SerializeField]
    private ScaleMode       scaleMode;
    [SerializeField, ShowIf(nameof(isScaleWithCharacter))]
    private ProtesterDef    characterType;
    [SerializeField]
    private Stat            stat;
    [SerializeField]
    private float           baseValue;
    [SerializeField, ShowIf(nameof(hasScale))]
    private float           multiplier;

    bool hasScale => (scaleMode == ScaleMode.ScaleWithCrowd) || (scaleMode == ScaleMode.ScaleWithType);
    bool isScaleWithCharacter => scaleMode == ScaleMode.ScaleWithType;

    public override string GetTooltip(IActionProvider mainObject)
    {
        float v = GetValue(mainObject);
        if (Mathf.Approximately(v, 0.0f)) return null;

        string timeString = GetClockString();

        if (v > 0) return $"<color=#{stat.color.ToHex()}>+{Mathf.RoundToInt(v)} {stat.displayName}{timeString}</color>";
        else return $"<color=#{stat.color.ToHex()}>{Mathf.RoundToInt(v)} {stat.displayName}{timeString}</color>";
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
}
