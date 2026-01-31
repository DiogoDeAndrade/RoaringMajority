using System;
using System.Collections.Generic;
using UC;
using UnityEngine;

[CreateAssetMenu(fileName = "News", menuName = "RM/News")]
public class News : ScriptableObject
{
    [Serializable]
    public struct Item
    {
        public bool     causeFaction;
        public string   newsItem;
    }

    public List<Item>   items = new();

    public string GetRandom()
    {
        return items.Random().newsItem;
    }

    public string GetRandom(bool proSide)
    {
        var filtered = items.FindAll(i => i.causeFaction == proSide);

        if (filtered.Count == 0)
        {
            return string.Empty;
        }

        return filtered[UnityEngine.Random.Range(0, filtered.Count)].newsItem;
    }
}
