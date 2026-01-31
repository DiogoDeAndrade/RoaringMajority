using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProtesterDef", menuName = "RM/ProtesterDef")]
public class ProtesterDef : ScriptableObject
{
    [System.Serializable]
    public struct BodySprite
    {
        public Sprite   bodySprite;
        public Vector3  offset;
    }

    public string           displayName;
    public Sprite           icon;
    public Color            color = Color.white;
    [TextArea]
    public string           tooltipText;
    public List<BodySprite> bodySprites;
    public List<Sprite>     maskSprites;
    [Header("Stats")]
    public int                  cost = 1;
    [SerializeReference]
    public List<UpkeepFunction> upkeepFunctions;

    public bool CanSpawn()
    {
        if (cost > GameManager.instance.availablePP) return false;

        return true;
    }

    public void GetUpkeep(Dictionary<Stat, float> deltaStat, ProtesterData protesterData)
    {
        foreach (var upkeepFunction in upkeepFunctions)
        {
            upkeepFunction.RunUpkeep(deltaStat, protesterData);
        }
    }
}
