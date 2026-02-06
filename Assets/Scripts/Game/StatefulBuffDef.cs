using NaughtyAttributes;
using System;
using UnityEngine;

public abstract class BuffDef : ScriptableObject 
{
    public abstract class BaseBuffData
    {
        public BuffDef          def;
        public IActionProvider  caster;
        public float            duration;
        public float            tickTime;

        public bool isTurnBased => def.isTurnBased;
        public bool isRealtime => def.isRealtime;

        public bool Tick(IActionProvider executor)
        {
            bool ret = true;
            if (isTurnBased)
            {
                duration -= 1.0f;

                ret = Run(executor);
            }
            else
            {
                tickTime -= Time.deltaTime;
                duration -= Time.deltaTime;

                if (tickTime <= 0.0f)
                {
                    tickTime += def.tickTime;
                    ret = Run(executor);
                }
            }

            return ret && (duration > 0.0f);
        }

        public abstract bool Run(IActionProvider executor);
        public abstract string GetTooltip(Stat stat, IActionProvider executor);
    }

    public enum ClockType { Realtime, TurnBased };

    [SerializeField] 
    protected string       _displayName;
    [SerializeField] 
    protected ClockType    _clockType;
    [SerializeField, ShowIf(nameof(isTurnBased))] 
    protected int          _durationTicks;
    [SerializeField, ShowIf(nameof(isRealtime))] 
    protected float        _durationTime;
    [SerializeField, ShowIf(nameof(isRealtime))]
    protected float        _tickTime;

    public bool isTurnBased => _clockType == ClockType.TurnBased;
    public bool isRealtime => _clockType == ClockType.Realtime;
    public float tickTime => _tickTime;

    public abstract bool isStatDelta(IActionProvider mainObject);
    public abstract string GetTooltip(IActionProvider mainObject);
    public abstract BaseBuffData Instance(IActionProvider mainObject);

    protected string GetClockString()
    {
        if (isTurnBased) return $" each turn, for {_durationTicks} turns";
        else if (isRealtime) return $" each {_tickTime}s for {Mathf.RoundToInt(_durationTime)}s";
        else return "";
    }
}

public abstract class StatefulBuffDef<T> : BuffDef where T : StatefulBuffDef<T>.BaseBuffData, new()
{
    public override BaseBuffData Instance(IActionProvider mainObject)
    {
        var ret = new T()
        {
            def = this,
            caster = mainObject,
            duration = isRealtime ? _durationTime : _durationTicks,
            tickTime = isRealtime ? _tickTime : 1.0f
        };

        return ret;
    }
}
