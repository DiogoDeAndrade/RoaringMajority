using System;
using System.Collections.Generic;
using UC;
using UnityEngine;

public interface IActionProvider
{
    public abstract LocationData GetLocation();
    public abstract Protester GetProtester();
}

[Serializable]
public abstract class CostFunction
{
    public abstract (Stat stat, float value) GetCost(IActionProvider mainObject);
    public abstract bool isStatDelta(IActionProvider mainObject);
    public abstract string GetTooltip(IActionProvider mainObject);
}

[Serializable]
public abstract class ConditionFunction
{
    public abstract bool Evaluate(IActionProvider mainObject);
    public abstract string GetTooltip(IActionProvider mainObject);
}

[Serializable]
public abstract class CooldownFunction
{
    public abstract float GetCooldown(IActionProvider mainObject);
}


[Serializable]
public abstract class ExecutionFunction
{
    public abstract bool Execute(IActionProvider mainObject);
    public abstract bool isStatDelta(IActionProvider mainObject);
    public abstract string GetTooltip(IActionProvider mainObject);
}

[Serializable]
[PolymorphicName("Default")]
public class ActionFunction
{
    [SerializeField] 
    private string _displayName;
    [SerializeField] 
    private Sprite _displayIcon;
    [SerializeField]
    private string _tooltipName;
    [SerializeField, TextArea] 
    private string _tooltipText;

    [SerializeReference]
    private CooldownFunction        cooldown;
    [SerializeReference]
    private List<CostFunction>      costs;
    [SerializeReference]
    private List<ConditionFunction> conditions;
    [SerializeReference]
    private List<ExecutionFunction> actions;

    public string displayName => _displayName;
    public string tooltipName => string.IsNullOrEmpty(_tooltipName) ? displayName : _tooltipName;
    public Sprite displayIcon => _displayIcon;
    public string tooltipText => _tooltipText;

    public virtual float CanRun(IActionProvider mainObject)
    {
        foreach (var cost in costs)
        {
            var c = cost.GetCost(mainObject);
            float currentValue = GameManager.instance.Get(c.stat);

            if (currentValue < c.value) return 0.0f; 
        }

        if (conditions != null)
        {
            foreach (var condition in conditions)
            {
                if (!condition.Evaluate(mainObject)) return 0.0f;
            }
        }

        var location = mainObject.GetLocation();
        var protester = mainObject.GetProtester();
        var globalCooldown = location.GetCooldown($"GCD_{protester.protesterDef.displayName}");
        var cd = location.GetCooldown(displayName);

        if (cd.cooldown < globalCooldown.cooldown) cd = globalCooldown;

        return 1.0f - Mathf.Clamp01(cd.cooldown / cd.maxCooldown);
    }

    public virtual void PayCost(IActionProvider mainObject)
    {
        foreach (var cost in costs)
        {
            var c = cost.GetCost(mainObject);
            float currentValue = GameManager.instance.Get(c.stat);

            GameManager.instance.Set(c.stat, currentValue - c.value);
        }
    }

    public void RunAction(IActionProvider mainObject)
    {
        if (RunActionActual(mainObject))
        {
            PayCost(mainObject);

            var location = mainObject.GetLocation();

            if (cooldown != null)
            {
                location?.SetCooldown(_displayName, cooldown.GetCooldown(mainObject));
            }

            var protester = mainObject.GetProtester();
            location?.SetCooldown($"GCD_{protester.protesterDef.displayName}", protester.protesterDef.globalCooldown);

            location?.NotifyAction();
        }
    }

    protected bool RunActionActual(IActionProvider mainObject)
    {
        bool ret = false;
        if ((actions != null) && (actions.Count > 0))
        {
            foreach (var e in actions)
            {
                ret |= e.Execute(mainObject);
            }
        }
        return ret;
    }

    public string GetDefaultCooldownTooltip(IActionProvider provider)
    {
        if (cooldown == null) return "";

        return $"{Mathf.FloorToInt(cooldown.GetCooldown(provider))}";
    }
    public string GetDefaultResultTooltip(IActionProvider provider)
    {
        string ret = "";

        foreach (var cost in costs)
        {
            if (cost.isStatDelta(provider))
            {
                var tmp = cost.GetTooltip(provider);
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (!string.IsNullOrEmpty(ret)) ret += ", ";
                    ret += tmp;
                }
            }
        }
        foreach (var action in actions)
        {
            if (action.isStatDelta(provider))
            {
                var tmp = action.GetTooltip(provider);
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (!string.IsNullOrEmpty(ret)) ret += ", ";
                    ret += tmp;
                }
            }
        }

        foreach (var action in actions)
        {
            if (!action.isStatDelta(provider))
            {
                var tmp = action.GetTooltip(provider);
                if (!string.IsNullOrEmpty(tmp))
                {
                    ret += tmp;
                }
            }
        }

        return ret;
    }

    public bool HasCooldown()
    {
        return (cooldown != null);
    }
}
