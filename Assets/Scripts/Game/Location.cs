using UnityEngine;
using System.Collections.Generic;
using System;
using UC;

[CreateAssetMenu(fileName = "Location", menuName = "RM/Location")]
public class Location : ScriptableObject
{
    [Serializable]
    public class Opposition
    {
        public Vector2Int           tensionRange;
        public Vector2Int           protesterCounterRange;    
        public List<ProtesterDef>   archetypes;
        public StringProbList       ticketText;
    }

    public string                   displayName;
    public Sprite                   backgroundImage;
    public string                   newsName;
    public LocationObject           prefab;
    [SerializeReference]
    public List<UpkeepFunction>     upkeepFunctions;
    [SerializeReference]
    public List<ActionFunction>     actions;
    public List<LocationVictory>    victoryConditions;
    public List<Opposition>         opposition;

    public void GetUpkeep(Dictionary<Stat, float> deltaStat, LocationData locationData)
    {
        foreach (var upkeepFunction in upkeepFunctions)
        {
            upkeepFunction.RunUpkeep(deltaStat, locationData);
        }
    }

    public float GetUpkeep(Stat stat, LocationData locationData)
    {
        float ret = 0.0f;

        foreach (var upkeepFunction in upkeepFunctions)
        {
            ret += upkeepFunction.GetUpkeep(stat, locationData);
        }

        return ret;
    }

    public LocationVictory GetVictoryCondition(LocationData location)
    {
        if ((victoryConditions != null) && (victoryConditions.Count > 0))
        {
            foreach (var victoryCondition in victoryConditions)
            {
                if (victoryCondition.CheckVictory(location))
                {
                    return victoryCondition;
                }
            }
        }
        return null;
    }

    public Opposition GetOpposition(float tension)
    {
        if (opposition == null) return null;

        foreach (var op in opposition)
        {
            if ((tension >= op.tensionRange.x) && (tension <= op.tensionRange.y))
            {
                return op;
            }
        }

        return null;
    }
}
