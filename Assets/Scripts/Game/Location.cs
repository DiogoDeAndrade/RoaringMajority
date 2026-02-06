using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Location", menuName = "RM/Location")]
public class Location : ScriptableObject
{
    public string                   displayName;
    public Sprite                   backgroundImage;
    public string                   newsName;
    public LocationObject           prefab;
    [SerializeReference]
    public List<UpkeepFunction>     upkeepFunctions;
    [SerializeReference]
    public List<ActionFunction>     actions;
    public List<LocationVictory>    victoryConditions;

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
}
