using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Location", menuName = "RM/Location")]
public class Location : ScriptableObject
{
    public string               displayName;
    public Sprite               backgroundImage;
    public string               newsName;
    [SerializeReference]
    public List<UpkeepFunction> upkeepFunctions;

    public void GetUpkeep(Dictionary<Stat, float> deltaStat, LocationData locationData)
    {
        foreach (var upkeepFunction in upkeepFunctions)
        {
            upkeepFunction.RunUpkeep(deltaStat, locationData);
        }
    }
}
