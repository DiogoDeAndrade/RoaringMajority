using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProtesterDef", menuName = "RM/ProtesterDef")]
public class ProtesterDef : ScriptableObject
{
    [Serializable]
    public class BodySprite
    {
        public Sprite   bodySprite;
        public Vector3  offset;
    }

    public string                   displayName;
    public Sprite                   icon;
    public Color                    color = Color.white;
    public Protester                protesterPrefab;
    [TextArea]
    public string                   tooltipText;
    public List<BodySprite>         bodySprites;
    public List<Sprite>             maskSprites;
    [Header("Stats")]
    public int                      cost = 1;
    public float                    globalCooldown = 30.0f;
    [SerializeReference]
    public List<ConditionFunction>  conditions;
    [SerializeReference]
    public List<UpkeepFunction>     upkeepFunctions;
    [SerializeReference]
    public List<ActionFunction>     actions;

    public bool CanSpawn(IActionProvider actionProvider)
    {
        if (cost > GameManager.instance.availablePP) return false;

        foreach (var condition in conditions)
        {
            if (!condition.Evaluate(actionProvider)) return false;
        }

        return true;
    }

    public void GetUpkeep(Dictionary<Stat, float> deltaStat, ProtesterData protesterData)
    {
        foreach (var upkeepFunction in upkeepFunctions)
        {
            upkeepFunction.RunUpkeep(deltaStat, protesterData);
        }
    }

    public float GetUpkeep(Stat stat, ProtesterData protesterData)
    {
        float ret = 0.0f;
        foreach (var upkeepFunction in upkeepFunctions)
        {
            ret += upkeepFunction.GetUpkeep(stat, protesterData);
        }

        return ret;
    }

    public string GetRequirementsTooltip(IActionProvider actionProvider)
    {
        string ret = null;
        foreach (var condition in conditions)
        {
            var tooltip = condition.GetTooltip(actionProvider);
            if (!string.IsNullOrEmpty(tooltip))
            {
                if (string.IsNullOrEmpty(ret)) ret = tooltip;
                else ret += $", {tooltip}";
            }
        }

        return ret;
    }
}
