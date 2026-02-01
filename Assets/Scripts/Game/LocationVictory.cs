using UnityEngine;

[CreateAssetMenu(fileName = "LocationVictory", menuName = "RM/Location Victory")]
public class LocationVictory : ScriptableObject
{
    [SerializeReference]
    private ConditionFunction[] conditions;
    [TextArea]
    public string text;

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

}
