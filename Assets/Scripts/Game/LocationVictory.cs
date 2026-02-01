using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationVictory", menuName = "RM/Location Victory")]
public class LocationVictory : ScriptableObject
{
    [SerializeReference]
    private ConditionFunction[] conditions;
    [TextArea]
    public string text;
    [SerializeReference]
    private ExecutionFunction[] actions;

    public bool CheckVictory(LocationData location)
    {
        foreach (var condition in conditions)
        {
            if (!condition.Evaluate(location))
            {
                return false;
            }
        }
        return true;
    }

    public bool RunAction()
    {
        bool ret = false;
        if ((actions != null) && (actions.Length > 0))
        {
            foreach (var e in actions)
            {
                ret |= e.Execute(GameManager.instance);
            }
        }
        return ret;

    }
}
